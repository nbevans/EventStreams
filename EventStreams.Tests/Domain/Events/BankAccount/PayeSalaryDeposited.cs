using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Events.BankAccount
{
    [DataContract]
    public class PayeSalaryDeposited : Credited {

        [DataMember]
        public string Source { get; private set; }

        public PayeSalaryDeposited(decimal takeHomeValue, string source)
            : base(takeHomeValue) {

            Source = source;
        }
    }
}
