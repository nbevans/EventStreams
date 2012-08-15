using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;

    public class EventStreamWriter : IDisposable {

        private static readonly int _hashHeaderPrefixLength =
            "Hash:  ".Length;

        private static readonly int _hashBase64Length =
            ((new SHA1Managed().HashSize / 8) + 2) / 3 * 4;

        private static readonly byte[] _separatorBytes =
            Encoding.UTF8.GetBytes("\r\n");

        private readonly Stream _innerStream;
        private readonly IEventWriter _eventWriter;

        public EventStreamWriter(Stream innerStream, IEventWriter eventWriter) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (eventWriter == null) throw new ArgumentNullException("eventWriter");
            _innerStream = innerStream;
            _eventWriter = eventWriter;
        }

        ~EventStreamWriter() {
            Dispose(false);
        }

        public void Flush() {
            _innerStream.Flush();
        }

        public void Write(IEnumerable<IStreamedEvent> streamedEvents) {
            var previousHash = ReadHashSeedOrNull();

            foreach (var se in streamedEvents) {
                using (var hashAlgo = new SHA1Managed())
                using (var cryptoStream = new CryptoStream(new NonClosingStreamWrapper(_innerStream), hashAlgo, CryptoStreamMode.Write)) {
                    InjectHashSeed(hashAlgo, previousHash);

                    _eventWriter.Write(cryptoStream, se.Arguments);

                    WriteSeparator(cryptoStream);
                    WriteHeader(cryptoStream, "Id", se.Id.ToString());
                    WriteHeader(cryptoStream, "Timestamp", se.Timestamp.ToString("O"));
                    WriteHeader(cryptoStream, "Type", se.Arguments.GetType().AssemblyQualifiedName);
                    
                    cryptoStream.FlushFinalBlock();
                    previousHash = hashAlgo.Hash;
                    WriteHeader(cryptoStream, "Hash", Convert.ToBase64String(previousHash));

                    WriteSeparator(cryptoStream);
                }
            }
        }

        private byte[] ReadHashSeedOrNull() {
            var peekBackLength = _hashHeaderPrefixLength + _hashBase64Length + (_separatorBytes.Length * 2);
            var peekBackPosition = _innerStream.Position - peekBackLength;
            var restorePosition = _innerStream.Position;

            if (peekBackPosition < 0)
                return null;

            try {
                _innerStream.Position = peekBackPosition;

                var buffer = new byte[_hashHeaderPrefixLength + _hashBase64Length];
                int bytesRead;
                if ((bytesRead = _innerStream.Read(buffer, 0, buffer.Length)) != buffer.Length)
                    throw new InvalidOperationException(
                        string.Format(
                            "An unexpected number of bytes were read from the stream. Expected {0}, but got {1}.",
                            buffer.Length,
                            bytesRead));

                if (buffer[0] != 'H' || buffer[1] != 'a' || buffer[2] != 's' || buffer[3] != 'h' ||
                    buffer[4] != ':' || buffer[5] != ' ' || buffer[6] != ' ')
                    throw new InvalidOperationException("The buffer read from the stream does not start with a hash prefix.");

                var str = Encoding.UTF8.GetString(buffer, _hashHeaderPrefixLength, _hashBase64Length);
                return Convert.FromBase64String(str);

            } finally {
                _innerStream.Position = restorePosition;
            }
        }

        private void InjectHashSeed(ICryptoTransform cryptoTransform, byte[] seedHash) {
            if (seedHash == null || seedHash.Length == 0)
                return;

            var numBytes = cryptoTransform.TransformBlock(seedHash, 0, seedHash.Length, null, 0);
            if (numBytes != seedHash.Length)
                throw new InvalidOperationException("The seed hash was injected but the number of bytes written does not match the number of bytes injected.");
        }

        private void WriteHeader(Stream stream, string name, string value) {
            var line = string.Concat(name, ":  ", value, Environment.NewLine);
            var bytes = Encoding.UTF8.GetBytes(line);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void WriteSeparator(Stream stream) {
            stream.Write(_separatorBytes, 0, _separatorBytes.Length);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing)
                GC.SuppressFinalize(this);

            _innerStream.Dispose();
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
