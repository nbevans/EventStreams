using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Events.BankAccount {
    [DataContract(Namespace = "")]
    public class Debited : EventArgs {

        [DataMember]
        public decimal Value { get; private set; }

        public Debited(decimal value) {
            Value = value;
        }
    }
}
