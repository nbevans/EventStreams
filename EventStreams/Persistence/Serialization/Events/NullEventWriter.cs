using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Events {
    public sealed class NullEventWriter : IEventWriter {
        public IEventReader Opposite { get { return new NullEventReader(); } }

        public void Write(Stream innerStream, EventArgs args) {
            innerStream.Write(new[] { (byte)'{', (byte)' ', (byte)'}' }, 0, 3);
        }
    }
}
