using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

        public IEnumerable<IStreamedEvent> Read() {
            using (var hashAlgo = new ShaHash())
            using (var cryptoStream = new CryptoStream(new NonClosingStream(_innerStream), hashAlgo, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cryptoStream, Encoding.UTF8, false, 1)) {

                var id = sr.ReadLine();
                var ts = sr.ReadLine();
                var ty = sr.ReadLine(); // streamreader is broken, its buffering reads too far ahead and screws up the hash block transform.
                var ag = sr.ReadLine();

                var pos = _innerStream.Position;

                cryptoStream.FlushFinalBlock();
                var hash = Convert.ToBase64String(hashAlgo.Hash);
            }

            return Enumerable.Empty<IStreamedEvent>();
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

            public ReadContext(Stream stream) {
                _stream = stream;
            }

        }
    }
}
