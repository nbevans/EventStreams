using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Events.BankAccount {
    [DataContract]
    public class Debited : EventArgs {

        [DataMember]
        public decimal Value { get; private set; }

        public Debited(decimal value) {
            Value = value;
        }
    }
}
