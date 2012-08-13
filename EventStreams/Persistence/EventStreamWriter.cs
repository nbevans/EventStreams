using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;

    public class EventStreamWriter : IDisposable {

        private static readonly byte[] _separatorBytes =
            Encoding.UTF8.GetBytes(Environment.NewLine + Environment.NewLine);

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
            foreach (var se in streamedEvents.ToArray()) {

                WriteHeader("Id", se.Id.ToString());
                WriteHeader("Timestamp", se.Timestamp.ToString("O"));
                WriteHeader("Type", se.Arguments.GetType().AssemblyQualifiedName);

                _eventWriter.Write(_innerStream, se.Arguments);

                WriteSeparator();
            }
        }

        private void WriteHeader(string name, string value) {
            var line = string.Concat(name, ":  ", value, Environment.NewLine);
            var bytes = Encoding.UTF8.GetBytes(line);
            _innerStream.Write(bytes, 0, bytes.Length);
        }

        private void WriteSeparator() {
            _innerStream.Write(_separatorBytes, 0, _separatorBytes.Length);
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
