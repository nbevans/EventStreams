using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;

    public class EventStreamReader : IDisposable {

        private readonly Stream _innerStream;
        private readonly IEventReader _eventReader;

        public EventStreamReader(Stream innerStream, IEventReader eventReader) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (eventReader == null) throw new ArgumentNullException("eventReader");
            _innerStream = innerStream;
            _eventReader = eventReader;
        }

        ~EventStreamReader() {
            Dispose(false);
        }

        public IStreamedEvent Next() {
            var previousHash =
                new EventStreamBacktracker(_innerStream)
                    .HashOrNull();

            using (var hashAlgo = new ShaHash())
            using (var cryptoStream = new CryptoStream(new NonClosingStream(_innerStream), hashAlgo, CryptoStreamMode.Read)) {
                var rc = new ReadContext(_innerStream, cryptoStream, hashAlgo); {

                    rc.Seed(previousHash);
                    rc.Header();
                    rc.Body(_eventReader);
                    rc.Footer();

                    previousHash = rc.StreamHash;
                    if (!previousHash.SequenceEqual(rc.CurrentHash))
                        throw new InvalidOperationException("Hash mismatch.");

                    return rc.Event;
                }
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing)
                GC.SuppressFinalize(this);

            _innerStream.Dispose();
        }

        public void Dispose() {
            Dispose(true);
        }

        private sealed class ReadContext {
            private readonly Stream _stream;
            private readonly CryptoStream _cryptoStream;
            private readonly HashAlgorithm _hashAlgo;
            private readonly BinaryReader _cryptoBinaryReader;
            private readonly BinaryReader _rawBinaryReader;
            private readonly TemporaryContainer _tempContainer;

            public IStreamedEvent Event { get { return _tempContainer.Build(); } }
            public byte[] StreamHash { get; private set; }
            public byte[] CurrentHash { get { return _tempContainer.Hash; } }

            public ReadContext(Stream stream, CryptoStream cryptoStream, HashAlgorithm hashAlgo) {
                _stream = stream;
                _cryptoStream = cryptoStream;
                _hashAlgo = hashAlgo;

                _rawBinaryReader = stream.ForBinaryReading();
                _cryptoBinaryReader = cryptoStream.ForBinaryReading();
                _tempContainer = new TemporaryContainer();
            }

            public void Seed(byte[] hash) {
                if (hash == null || hash.Length == 0)
                    return;

                var numBytes = _hashAlgo.TransformBlock(hash, 0, hash.Length, null, 0);
                if (numBytes != hash.Length)
                    throw new InvalidOperationException("The seed hash was injected but the number of bytes written does not match the number of bytes injected.");
            }

            public void Header() {
                if (_cryptoBinaryReader.ReadByte() != EventStreamTokens.RecordStartIndicator)
                    throw new InvalidOperationException("The stream is not positioned at the start of a record as the indicator byte is not present.");

                _tempContainer.HeadRecordLength = _cryptoBinaryReader.ReadInt32();
                _tempContainer.Id = new Guid(_cryptoBinaryReader.ReadBytes(16));
                _tempContainer.Timestamp = new DateTime(_cryptoBinaryReader.ReadInt64(), DateTimeKind.Utc);
                _tempContainer.ArgumentsType = Type.GetType(_cryptoBinaryReader.ReadString(), true);
            }

            public void Body(IEventReader eventReader) {
                Debug.Assert(_tempContainer.ArgumentsType != null);

                var bodyLength = _cryptoBinaryReader.ReadInt32();
                var positionBefore = _stream.Position;

                _tempContainer.Arguments = eventReader.Read(_cryptoStream, _tempContainer.ArgumentsType);

                if (positionBefore + bodyLength != _stream.Position)
                    throw new InvalidOperationException("The stream has advanced further than expected whilst reading the body stream; it may be invalid, malformed or corrupt.");
            }

            public void Footer() {
                FinaliseHash();

                // Must not use _cryptoBinaryReader or _cryptoStream now that the hash has been finalised.
                // Use _rawBinaryReader from this point on.

                _tempContainer.Hash = _rawBinaryReader.ReadBytes(ShaHash.ByteLength);
                _tempContainer.TailRecordLength = _rawBinaryReader.ReadInt32();

                if (_rawBinaryReader.ReadByte() != EventStreamTokens.RecordEndIndicator)
                    throw new InvalidOperationException("The stream has reached the end of the current record but an indicator byte is not present.");

                if (_tempContainer.HeadRecordLength != _tempContainer.TailRecordLength)
                    throw new InvalidOperationException("The head and tail record length indicators are different; the stream may be invalid, malformed or corrupt.");
            }

            private void FinaliseHash() {
                _cryptoStream.FlushFinalBlock();
                StreamHash = _hashAlgo.Hash;
                Debug.Assert(StreamHash.Length == ShaHash.ByteLength);
            }

            private sealed class TemporaryContainer {
                public int HeadRecordLength; // verify this turns out to be correct i.e. compare before and after stream positions
                public Guid Id;
                public DateTime Timestamp;
                public Type ArgumentsType;
                public EventArgs Arguments;
                public byte[] Hash;
                public int TailRecordLength;

                public IStreamedEvent Build() {
                    return new StreamedEvent(Id, Timestamp, Arguments);
                }
            }
        }
    }
}
