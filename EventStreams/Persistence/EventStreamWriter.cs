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
            ((new ShaHash().HashSize / 8) + 2) / 3 * 4;

        private static readonly byte[] _newLineBytes =
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
                using (var hashAlgo = new ShaHash())
                using (var cryptoStream = new CryptoStream(new NonClosingStreamWrapper(_innerStream), hashAlgo, CryptoStreamMode.Write)) {
                    InjectHashSeed(hashAlgo, previousHash);

                    var wc = new WriteContext(cryptoStream, se); {
                        wc.Header("Id", se.Id.ToString());
                        wc.Header("Time", se.Timestamp.ToString("O"));
                        wc.Header("Type", se.Arguments.GetType().AssemblyQualifiedName);
                        wc.Header("Args", (s, e) => _eventWriter.Write(s, e.Arguments));

                        cryptoStream.FlushFinalBlock();
                        previousHash = hashAlgo.Hash;
                        wc.Header("Hash", Convert.ToBase64String(previousHash));
                        wc.Separator();
                    }
                }
            }
        }

        private byte[] ReadHashSeedOrNull() {
            var peekBackLength = _hashHeaderPrefixLength + _hashBase64Length + (_newLineBytes.Length * 2);
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

        protected virtual void Dispose(bool disposing) {
            if (disposing)
                GC.SuppressFinalize(this);

            _innerStream.Dispose();
        }

        public void Dispose() {
            Dispose(true);
        }

        private sealed class WriteContext {
            private readonly Stream _stream;
            private readonly IStreamedEvent _streamedEvent;

            public WriteContext(Stream stream, IStreamedEvent streamedEvent) {
                _stream = stream;
                _streamedEvent = streamedEvent;
            }

            public void Header(string name, Action<Stream, IStreamedEvent> valueWriter) {
                var line = string.Concat(name, ":", GetSpaces(name));
                var bytes = Encoding.UTF8.GetBytes(line);
                _stream.Write(bytes, 0, bytes.Length);
                valueWriter(_stream, _streamedEvent);
                Separator();
            }

            public void Header(string name, string value) {
                var line = string.Concat(name, ":", GetSpaces(name), value, "\r\n");
                var bytes = Encoding.UTF8.GetBytes(line);
                _stream.Write(bytes, 0, bytes.Length);
            }

            public void Separator() {
                _stream.Write(_newLineBytes, 0, _newLineBytes.Length);
            }

            private static string GetSpaces(string name) {
                return new string(' ', Math.Max(7 - name.Length - 1, 2));
            }
        }
    }
}
