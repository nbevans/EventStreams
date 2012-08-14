using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Events {
    public sealed class NullEventWriter : IEventWriter {
        public void Write(Stream innerStream, EventArgs args) {
            innerStream.Write(new[] { (byte)'{', (byte)' ', (byte)'}' }, 0, 3);
        }
    }
}
