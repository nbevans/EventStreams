using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;

    public class EventStreamWriter : IDisposable {

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
            var previousHash =
                new EventStreamBacktracker(_innerStream)
                    .HashSeedOrNull();

            foreach (var se in streamedEvents) {
                using (var hashAlgo = new ShaHash())
                using (var cryptoStream = new CryptoStream(new NonClosingStreamWrapper(_innerStream), hashAlgo, CryptoStreamMode.Write)) {
                    InjectHashSeed(hashAlgo, previousHash);

                    var wc = new WriteContext(cryptoStream, se); {
                        wc.Header(EventStreamTokens.Id, se.Id.ToString());
                        wc.Header(EventStreamTokens.Time, se.Timestamp.ToString("O"));
                        wc.Header(EventStreamTokens.Type, se.Arguments.GetType().AssemblyQualifiedName);
                        wc.Header(EventStreamTokens.Args, (s, e) => _eventWriter.Write(s, e.Arguments));

                        cryptoStream.FlushFinalBlock();
                        previousHash = hashAlgo.Hash;
                        wc.Header(EventStreamTokens.Hash, Convert.ToBase64String(previousHash));
                        wc.Separator();
                    }
                }
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
                var line = string.Concat(name, EventStreamTokens.HeaderSuffix, GetSpaces(name));
                var bytes = Encoding.UTF8.GetBytes(line);
                _stream.Write(bytes, 0, bytes.Length);
                valueWriter(_stream, _streamedEvent);
                Separator();
            }

            public void Header(string name, string value) {
                var line = string.Concat(name, EventStreamTokens.HeaderSuffix, GetSpaces(name), value, EventStreamTokens.NewLine);
                var bytes = Encoding.UTF8.GetBytes(line);
                _stream.Write(bytes, 0, bytes.Length);
            }

            public void Separator() {
                _stream.Write(EventStreamTokens.NewLineBytes, 0, EventStreamTokens.NewLineBytes.Length);
            }

            private static string GetSpaces(string name) {
                return new string(EventStreamTokens.HeaderSuffixWhitespace, Math.Max(7 - name.Length - 1, 2));
            }
        }
    }
}
