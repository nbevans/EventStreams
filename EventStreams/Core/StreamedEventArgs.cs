using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace EventStreams.Core
{
    [DataContract]
    [DebuggerDisplay("{DebugName}")]
    public abstract class StreamedEventArgs : EventArgs, IStreamedEvent {
        
        [DataMember(Order = 0)]
        public Guid Id { get; private set; }

        [DataMember(Order = 1)]
        public DateTime Timestamp { get; private set; }

        protected StreamedEventArgs()
        {
            Id = Guid.NewGuid();
            Timestamp = TimeSource.UtcNow;
        }

        public string DebugName
        {
            get { return string.Format("#{0:x} @ {1:o}", Id.GetHashCode(), Timestamp); }
        }
    }
}
