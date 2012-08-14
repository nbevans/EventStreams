using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain {

    [DataContract]
    public class BankAccountState {

        [DataMember(Order = 1)]
        public decimal Balance { get; set; }
    }
}
