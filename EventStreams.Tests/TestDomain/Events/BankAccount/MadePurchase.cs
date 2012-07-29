using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events.BankAccount
{
    [DataContract]
    public class MadePurchase : Debited {

        [DataMember]
        public string Name { get; private set; }

        public MadePurchase(decimal value, string name)
            : base(value) {

            Name = name;
        }
    }
}
