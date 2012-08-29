using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Events {
    public sealed class NullEventReader : IEventReader {
        public IEventWriter Opposite { get { return new NullEventWriter(); } }

        public EventArgs Read(Stream innerStream, Type concreteType) {
            var buffer = new byte[3];
            if (innerStream.Read(buffer, 0, 3) != 3 || buffer[0] != (byte)'{' || buffer[1] != (byte)' ' || buffer[2] != (byte)'}')
                throw new InvalidOperationException(
                    string.Format(
                        "The stream did not contain the expected data at the current position. " +
                        "Please ensure that the data being read was originally output by {0}.",
                        typeof(NullEventWriter).Name));

            return (EventArgs)Activator.CreateInstance(concreteType);
        }
    }
}
