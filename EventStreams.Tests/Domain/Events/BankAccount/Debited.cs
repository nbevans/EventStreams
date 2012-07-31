using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Events.BankAccount
{
    using Core;

    [DataContract]
    public class Debited : StreamedEventArgs {

        [DataMember]
        public decimal Value { get; private set; }

        public Debited(decimal value) {
            Value = value;
        }
    }
}
