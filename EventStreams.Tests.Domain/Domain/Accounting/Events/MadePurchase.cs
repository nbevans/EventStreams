using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Accounting.Events {
    [DataContract(Namespace = "")]
    public class MadePurchase : Debited {

        [DataMember]
        public string Name { get; private set; }

        public MadePurchase() { }

        public MadePurchase(decimal value, string name)
            : base(value) {

            Name = name;
        }
    }
}
