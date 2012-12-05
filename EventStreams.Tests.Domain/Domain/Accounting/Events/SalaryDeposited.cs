using System;
using System.Runtime.Serialization;

namespace EventStreams.Domain.Accounting.Events {
    [DataContract(Namespace = "")]
    public class SalaryDeposited : Credited {

        public SalaryDeposited() { }

        public SalaryDeposited(decimal value)
            : base(value) { }
    }
}
