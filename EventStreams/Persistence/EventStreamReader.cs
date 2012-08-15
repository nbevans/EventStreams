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

        private static readonly byte[] _separatorBytes =
            Encoding.UTF8.GetBytes("\r\n");

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
            return Enumerable.Empty<IStreamedEvent>();
            //var previousHash = ReadHashSeedOrNull();

            //foreach (var se in streamedEvents) {
            //    using (var hashAlgo = new ShaHash())
            //    using (var cryptoStream = new CryptoStream(new NonClosingStreamWrapper(_innerStream), hashAlgo, CryptoStreamMode.Write)) {
            //        _eventReader.Read(cryptoStream, se.Arguments);

            //        WriteSeparator(cryptoStream);
            //        WriteHeader(cryptoStream, "Id", se.Id.ToString());
            //        WriteHeader(cryptoStream, "Timestamp", se.Timestamp.ToString("O"));
            //        WriteHeader(cryptoStream, "Type", se.Arguments.GetType().AssemblyQualifiedName);

            //        cryptoStream.FlushFinalBlock();
            //        previousHash = hashAlgo.Hash;
            //        WriteHeader(cryptoStream, "Hash", Convert.ToBase64String(previousHash));

            //        WriteSeparator(cryptoStream);
            //    }
            //}
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
