using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace EventStreams.Persistence.Serialization.Events {
    public sealed class XmlEventWriter : IEventWriter {
        private static readonly XmlWriterSettings _xmlWriterSettings =
            new XmlWriterSettings {
                CloseOutput = false,
                ConformanceLevel = ConformanceLevel.Fragment,
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

        public IEventReader Opposite { get { throw new NotImplementedException(); } }

        public void Write(Stream innerStream, EventArgs args) {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (args == null) throw new ArgumentNullException("args");

            using (var xw = XmlWriter.Create(innerStream, _xmlWriterSettings)) {
                new DataContractSerializer(args.GetType())
                    .WriteObject(xw, args);
            }
        }
    }
}
