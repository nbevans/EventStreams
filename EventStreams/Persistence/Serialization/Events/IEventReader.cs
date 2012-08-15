using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Events {
    public interface IEventReader {
        EventArgs Read(Stream innerStream, Type concreteType);
    }
}