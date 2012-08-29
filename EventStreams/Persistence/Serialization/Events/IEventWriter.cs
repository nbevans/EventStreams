using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Events {
    public interface IEventWriter {
        IEventReader Opposite { get; }
        void Write(Stream innerStream, EventArgs args);
    }
}