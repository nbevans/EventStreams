using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Events.BankAccount {
    [DataContract]
    public class SalaryDeposited : Credited {

        public SalaryDeposited(decimal value)
            : base(value) { }
    }
}
