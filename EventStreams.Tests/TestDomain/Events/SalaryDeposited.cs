using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events
{
    using Core;

    [DataContract]
    class SalaryDeposited : EventBase<BankAccount>
    {
        [DataMember]
        public decimal Value { get; private set; }

        public SalaryDeposited(decimal value)
        {
            Value = value;
            Aggregator = currentState => currentState.Credit(Value);
        }
    }
}
