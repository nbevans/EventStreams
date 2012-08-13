using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace EventStreams.Persistence {
    using Core;

    public class EventStreamWriter : IDisposable {

        private static readonly byte[] _separatorBytes =
            Encoding.UTF8.GetBytes(Environment.NewLine + Environment.NewLine);

        private static readonly XmlWriterSettings _xmlWriterSettings =
            new XmlWriterSettings {
                CloseOutput = false,
                ConformanceLevel = ConformanceLevel.Fragment,
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

        private readonly Stream _innerStream;

        public EventStreamWriter(Stream innerStream) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            _innerStream = new NonClosingStreamWrapper(innerStream);
        }

        ~EventStreamWriter() {
            Dispose(false);
        }

        public void Flush() {
            _innerStream.Flush();
        }

        public void Write(IEnumerable<IStreamedEvent> streamedEvents) {
            foreach (var se in streamedEvents) {
                    
                WriteHeader("Id", se.Id.ToString());
                WriteHeader("Timestamp", se.Timestamp.ToString("O"));

                using (var xw = XmlWriter.Create(_innerStream, _xmlWriterSettings)) {
                    new DataContractSerializer(se.Arguments.GetType())
                        .WriteObject(xw, se.Arguments);
                }

                WriteSeparator();
            }
        }

        private void WriteHeader(string name, string value) {
            var line = string.Concat(name, ": ", value, Environment.NewLine);
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
