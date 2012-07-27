using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events
{
    using Core;

    [DataContract]
    class PayeSalaryDeposited : EventBase<BankAccount>
    {
        [DataMember]
        public decimal TakeHomeValue { get; private set; }

        public PayeSalaryDeposited(decimal takeHomeValue)
        {
            TakeHomeValue = takeHomeValue;
            Aggregator = currentState => currentState.Credit(TakeHomeValue);
        }
    }
}
