using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events
{
    [DataContract]
    class CheeseProductPurchased : ProductPurchased
    {
        public CheeseProductPurchased(decimal value)
            : base(value)
        {
        }
    }
}
