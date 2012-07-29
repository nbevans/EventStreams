using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain {

    [DataContract]
    public class BankAccountState {

        [DataMember]
        public decimal Balance { get; set; }

    }
}
