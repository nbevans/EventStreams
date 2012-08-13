using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace EventStreams.Core {
    [DataContract]
    [DebuggerDisplay("{DebugName}")]
    public sealed class StreamedEvent : IStreamedEvent {

        [DataMember(Order = 1)]
        public Guid Id { get; private set; }

        [DataMember(Order = 2)]
        public DateTime Timestamp { get; private set; }

        [DataMember(Order = 3)]
        public EventArgs Arguments { get; private set; }

        public StreamedEvent(EventArgs arguments) {
            if (arguments == null) throw new ArgumentNullException("arguments");

            Id = Guid.NewGuid();
            Timestamp = TimeSource.UtcNow;
            Arguments = arguments;
        }

        public string DebugName {
            get { return string.Format("{0} (#{1:x}) @ {2:o}", Arguments.GetType().Name, Id.GetHashCode(), Timestamp); }
        }
    }
}
