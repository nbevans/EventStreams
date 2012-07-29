using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events.BankAccount
{
    using Core;

    [DataContract]
    public class Credited : StreamedEventArgs {

        [DataMember]
        public decimal Value { get; private set; }

        public Credited(decimal value) {
            Value = value;
        }
    }
}
