using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace EventStreams.Core {
    /// <summary>
    /// Acts as a container for a <see cref="EventArgs"/> object but with additional event sourcing meta data attached, including a unique identifier and timestamp.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugName}")]
    public sealed class StreamedEvent : IStreamedEvent {
        
        /// <summary>
        /// Gets the unique identifier of the event.
        /// </summary>
        [DataMember(Order = 1)]
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the timestamp of the event.
        /// </summary>
        /// <remarks>
        /// In the context of bi-temporal or multi-temporal models, this timestamp is considered to be the "event time" and not in anyway some form of "reality time".
        /// </remarks>
        [DataMember(Order = 2)]
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets the event arguments that describe the domain fact.
        /// </summary>
        [DataMember(Order = 3)]
        public EventArgs Arguments { get; private set; }

        public StreamedEvent(EventArgs arguments) : this(Guid.NewGuid(), TimeSource.UtcNow, arguments) { }

        public StreamedEvent(Guid identity, DateTime timestamp, EventArgs arguments) {
            if (arguments == null) throw new ArgumentNullException("arguments");

            Id = identity;
            Timestamp = timestamp;
            Arguments = arguments;
        }

        public string DebugName {
            get { return string.Format("{0} (#{1:x}) @ {2:o}", Arguments.GetType().Name, Id.GetHashCode(), Timestamp); }
        }
    }
}
