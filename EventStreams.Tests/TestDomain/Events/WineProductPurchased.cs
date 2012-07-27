using System;
using System.Runtime.Serialization;

namespace EventStreams.TestDomain.Events
{
    [DataContract]
    class WineProductPurchased : ProductPurchased
    {
        public WineProductPurchased(decimal value)
            : base(value)
        {
        }
    }
}
