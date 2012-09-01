using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace EventStreams.Persistence.Streams {
    using Core;
    using Resources;
    using Decorators;
    using Serialization.Events;

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
                throw new DataVerificationPersistenceException(
                    string.Format(ExceptionStrings.Seed_hash_is_invalid_length, ShaHash.ByteLength));

            var numBytes = _hashAlgo.TransformBlock(hash, 0, hash.Length, null, 0);
            if (numBytes != hash.Length)
                throw new DataVerificationPersistenceException(
                    ExceptionStrings.Seed_hash_injected_but_unexpected_number_of_written_bytes);
        }

        public void HeadIndicator() {
            if (_cryptoBinaryReader.ReadByte() != EventStreamTokens.HeadIndicator)
                throw new DataVerificationPersistenceException(
                    ExceptionStrings.Head_indicator_byte_not_present);
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

            _tempContainer.Arguments = eventReader.Read(_cryptoStream.VirtualLength(bodyLength), _tempContainer.ArgumentsType);

            if (positionBefore + bodyLength != _stream.Position)
                throw new DataVerificationPersistenceException(
                    ExceptionStrings.Body_length_indicator_mismatches_actual_body_length);
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
                throw new DataVerificationPersistenceException(
                    ExceptionStrings.Head_and_tail_indicators_mismatch);
        }

        public void TailIndicator() {
            if (_rawBinaryReader.ReadByte() != EventStreamTokens.TailIndicator)
                throw new DataVerificationPersistenceException(
                    ExceptionStrings.Tail_indicator_byte_not_present);
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
