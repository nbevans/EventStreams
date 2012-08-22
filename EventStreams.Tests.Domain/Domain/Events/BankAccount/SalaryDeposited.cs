using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Events.BankAccount {
    [DataContract(Namespace = "")]
    public class SalaryDeposited : Credited {

        public SalaryDeposited() { }

        public SalaryDeposited(decimal value)
            : base(value) { }
    }
}
