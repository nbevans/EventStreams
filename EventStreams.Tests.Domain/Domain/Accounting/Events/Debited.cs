using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Accounting.Events {
    [DataContract(Namespace = "")]
    public class Debited : EventArgs {

        [DataMember]
        public decimal Value { get; private set; }

        public Debited() { }

        public Debited(decimal value) {
            Value = value;
        }
    }
}
