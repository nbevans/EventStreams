using System;

namespace EventStreams.Core.Domain {
    public struct AggregateRootIdentity {
        public Guid Value { get; private set; }

        public AggregateRootIdentity(Guid value)
            : this() {

            Value = value;
        }
    }
}