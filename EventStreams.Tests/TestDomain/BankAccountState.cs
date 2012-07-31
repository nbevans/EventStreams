using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain {

    [DataContract]
    public class BankAccountState {

        [DataMember(Order = 1)]
        public decimal Balance { get; set; }

        [DataMember(Order = 2)]
        public string Foo { get; set; }

        [DataMember(Order = 3)]
        public DateTime Dt { get; set; }

    }
}
