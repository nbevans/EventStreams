using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Events {
    public interface IEventReader {
        IEventWriter Opposite { get; }
        EventArgs Read(Stream innerStream, Type concreteType);
    }
}