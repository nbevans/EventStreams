using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Events {
    public interface IEventWriter {
        void Write(Stream innerStream, EventArgs args);
    }
}