using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

using EventStreams.Core;
using EventStreams.Persistence.Serialization.Events;

namespace EventStreams.Persistence {

    internal sealed class EventStreamReaderContext : IEventStreamReaderContext {
        private static readonly ConcurrentDictionary<string, Type> _typeCache =
            new ConcurrentDictionary<string, Type>();

        private readonly Stream _stream;
        private readonly CryptoStream _cryptoStream;
        private readonly HashAlgorithm _hashAlgo;

        private readonly BinaryReader _cryptoBinaryReader;
        private readonly BinaryReader _rawBinaryReader;
        private readonly TemporaryContainer _tempContainer;

        public IStreamedEvent Event { get { return _tempContainer.Build(); } }
        public byte[] StreamHash { get; private set; }
        public byte[] CurrentHash { get { return _tempContainer.Hash; } }
        public long CurrentHashPosition { get; private set; }

        public EventStreamReaderContext(Stream stream, CryptoStream cryptoStream, HashAlgorithm hashAlgo, byte[] seedHash) {
            _stream = stream;
            _cryptoStream = cryptoStream;
            _hashAlgo = hashAlgo;

            _rawBinaryReader = stream.ForBinaryReading();
            _cryptoBinaryReader = cryptoStream.ForBinaryReading();
            _tempContainer = new TemporaryContainer();

            Seed(seedHash);
        }

        private void Seed(byte[] hash) {
            if (hash == null || hash.Length == 0)
                return;

            if (hash.Length != ShaHash.ByteLength)
                throw new InvalidOperationException(
                    string.Format(
                        "The seed hash is not of a valid length; it must be {0} bytes long.",
                        ShaHash.ByteLength));

            var numBytes = _hashAlgo.TransformBlock(hash, 0, hash.Length, null, 0);
            if (numBytes != hash.Length)
                throw new InvalidOperationException("The seed hash was injected but the number of bytes written does not match the number of bytes injected.");
        }

        public void HeadIndicator() {
            if (_cryptoBinaryReader.ReadByte() != EventStreamTokens.HeadIndicator)
                throw new InvalidOperationException("The stream is not positioned at the start of a record as the indicator byte is not present.");
        }

        public void HeadRecordLength() {
            _tempContainer.HeadRecordLength = _cryptoBinaryReader.ReadInt32();
        }

        public void Id() {
            _tempContainer.Id = new Guid(_cryptoBinaryReader.ReadBytes(16));
        }

        public void Timestamp() {
            _tempContainer.Timestamp = new DateTime(_cryptoBinaryReader.ReadInt64(), DateTimeKind.Utc);
        }

        public void ArgumentsType() {
            _tempContainer.ArgumentsType =
                _typeCache
                    .GetOrAdd(
                        _cryptoBinaryReader.ReadString(),
                        v => Type.GetType(v, true));
        }

        public void Body(IEventReader eventReader) {
            Debug.Assert(_tempContainer.ArgumentsType != null);

            var bodyLength = _cryptoBinaryReader.ReadInt32();
            var positionBefore = _stream.Position;

            _tempContainer.Arguments = eventReader.Read(_cryptoStream, _tempContainer.ArgumentsType);

            if (positionBefore + bodyLength != _stream.Position)
                throw new InvalidOperationException("The stream has advanced further than expected whilst reading the body stream; it may be invalid, malformed or corrupt.");
        }

        public void Hash() {
            _cryptoStream.FlushFinalBlock();

            // Must not use _cryptoBinaryReader or _cryptoStream now that the hash has been finalised.
            // Use _rawBinaryReader from this point on.

            StreamHash = _hashAlgo.Hash;
            Debug.Assert(StreamHash.Length == ShaHash.ByteLength);

            CurrentHashPosition = _stream.Position;
            _tempContainer.Hash = _rawBinaryReader.ReadBytes(ShaHash.ByteLength);
        }

        public void TailRecordLength() {
            _tempContainer.TailRecordLength = _rawBinaryReader.ReadInt32();
            if (_tempContainer.TailRecordLength != _tempContainer.HeadRecordLength)
                throw new InvalidOperationException("The head and tail record length values are different; the stream may be invalid, malformed or corrupt.");
        }

        public void TailIndicator() {
            if (_rawBinaryReader.ReadByte() != EventStreamTokens.TailIndicator)
                throw new InvalidOperationException("The stream has reached the end of the current record but a tail indicator byte is not present.");
        }

        private sealed class TemporaryContainer {
            public int HeadRecordLength;
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
