using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Events.BankAccount {
    [DataContract(Namespace = "")]
    public class Credited : EventArgs
    {

        [DataMember]
        public decimal Value { get; private set; }

        public Credited(decimal value) {
            Value = value;
        }
    }
}
