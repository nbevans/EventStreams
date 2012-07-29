using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events.BankAccount
{
    [DataContract]
    public class SalaryDeposited : Credited {

        public SalaryDeposited(decimal value)
            : base(value) { }
    }
}
