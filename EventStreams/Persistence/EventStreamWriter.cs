using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;
    using Resources;

    public class EventStreamWriter : IEventStreamWriter {

        private readonly Stream _innerStream;
        private readonly IEventWriter _eventWriter;

        public EventStreamWriter(Stream innerStream, IEventWriter eventWriter) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (eventWriter == null) throw new ArgumentNullException("eventWriter");

            _innerStream = innerStream;
            _eventWriter = eventWriter;

            Debug.Assert(innerStream.CanRead);
            Debug.Assert(innerStream.CanWrite);
            Debug.Assert(innerStream.CanSeek);
        }

        ~EventStreamWriter() {
            Dispose(false);
        }

        public Stream InnerStream { get { return _innerStream; } }
        public IEventWriter EventWriter { get { return _eventWriter; } }

        public void Write(IEnumerable<IStreamedEvent> streamedEvents) {
            var previousHash =
                new EventStreamBacktracker(_innerStream)
                    .HashOrNull();

            foreach (var se in streamedEvents) {
                using (var hashAlgo = new ShaHash())
                using (var cryptoStream = new CryptoStream(_innerStream.PreventClosure(), hashAlgo, CryptoStreamMode.Write)) {
                    var wc = new WriteContext(_innerStream, cryptoStream, hashAlgo, se); {
                        wc.Seed(previousHash);
                        wc.Body(_eventWriter);
                        wc.Header();
                        wc.Body();
                        wc.Footer();

                        previousHash = wc.StreamHash;
                    }
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

        private sealed class WriteContext {
            private readonly CryptoStream _cryptoStream;
            private readonly HashAlgorithm _hashAlgo;
            private readonly IStreamedEvent _streamedEvent;
            private readonly BinaryWriter _cryptoBinaryWriter;
            private readonly BinaryWriter _rawBinaryWriter;
            private readonly string _argumentsType;
            private byte[] _bodyBuffer;
            private int _bodyLength;

            public byte[] StreamHash { get; private set; }

            public WriteContext(Stream stream, CryptoStream cryptoStream, HashAlgorithm hashAlgo, IStreamedEvent streamedEvent) {
                _cryptoStream = cryptoStream;
                _hashAlgo = hashAlgo;
                _streamedEvent = streamedEvent;

                _rawBinaryWriter = stream.ForBinaryWriting();
                _cryptoBinaryWriter = cryptoStream.ForBinaryWriting();
                _argumentsType = _streamedEvent.Arguments.GetType().AssemblyQualifiedName;
            }

            public void Seed(byte[] hash) {
                if (hash == null || hash.Length == 0)
                    return;

                var numBytes = _hashAlgo.TransformBlock(hash, 0, hash.Length, null, 0);
                if (numBytes != hash.Length)
                    throw new DataVerificationPersistenceException(
                        ExceptionStrings.Seed_hash_injected_but_unexpected_number_of_written_bytes);
            }

            public void Header() {
                Debug.Assert(_bodyBuffer != null);

                _cryptoBinaryWriter.Write(EventStreamTokens.HeadIndicator);
                _cryptoBinaryWriter.Write(CalculateRecordLength());
                _cryptoBinaryWriter.Write(_streamedEvent.Id.ToByteArray());
                _cryptoBinaryWriter.Write(_streamedEvent.Timestamp.Ticks);
                _cryptoBinaryWriter.Write(_argumentsType);
            }

            public void Body(IEventWriter eventWriter) {
                using (var bodyStream = new MemoryStream(512)) {
                    eventWriter.Write(bodyStream, _streamedEvent.Arguments);

                    _bodyBuffer = bodyStream.GetBuffer();
                    _bodyLength = (int)bodyStream.Length;
                }
            }

            public void Body() {
                Debug.Assert(_bodyBuffer != null);

                _cryptoBinaryWriter.Write(_bodyLength);
                _cryptoBinaryWriter.Write(_bodyBuffer, 0, _bodyLength);
            }

            public void Footer() {
                FinaliseHash();

                // Must not use _cryptoBinaryReader or _cryptoStream now that the hash has been finalised.
                // Use _rawBinaryReader from this point on.

                _rawBinaryWriter.Write(StreamHash);
                _rawBinaryWriter.Write(CalculateRecordLength());
                _rawBinaryWriter.Write(EventStreamTokens.TailIndicator);
            }

            private int CalculateRecordLength() {
                Debug.Assert(_bodyBuffer != null);

                // Every record is laid out in the follow structure, and order.
                // We can therefore calculate precisely the record length before
                // it has even been (fully) written.
                // Note that the record length is written at the start *and* end.
                // This is to allow efficient record iteration in both directions.
                // It's basically a doubly linked list structure set within a
                // contiguous block of memory (and obviously we're optimising for
                // a file stream!)

                var len = 0;
                len += sizeof(int); // record length prefix (this!)
                len += 16; // guid
                len += sizeof(long); // timestamp
                len += _argumentsType.Length;
                len += sizeof(int); // the body length prefix
                len += _bodyLength; // the actual body
                len += ShaHash.ByteLength;
                len += sizeof(int); // record length suffix (this!)
                return len;
            }

            private void FinaliseHash() {
                _cryptoStream.FlushFinalBlock();
                StreamHash = _hashAlgo.Hash;
                Debug.Assert(StreamHash.Length == ShaHash.ByteLength);
            }
        }
    }
}
