using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events
{
    using Core;

    [DataContract]
    class ProductPurchased : EventBase<BankAccount>
    {
        [DataMember]
        public decimal Value { get; private set; }

        public ProductPurchased(decimal value)
        {
            Value = value;
            Aggregator = currentState => currentState.Debit(Value);
        }
    }
}
