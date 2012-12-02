using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain {

    [DataContract]
    public class BankAccountState {

        [IgnoreDataMember]
        public Guid Identity { get; private set; }

        [DataMember(Order = 1)]
        public decimal Balance { get; set; }

        public BankAccountState()
            : this(Guid.NewGuid()) { }

        public BankAccountState(Guid identity) {
            Identity = identity;
        }
    }
}
