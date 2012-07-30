using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain {

    [DataContract]
    public class BankAccountState {

        [DataMember(Order = 0)]
        public decimal Balance { get; set; }

        [DataMember(Order = 1)]
        public string Foo { get; set; }

    }
}
