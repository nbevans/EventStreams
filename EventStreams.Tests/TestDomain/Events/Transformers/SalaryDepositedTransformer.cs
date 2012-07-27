using System;
using System.Collections.Generic;

namespace EventStreams.TestDomain.Events.Transformers
{
    using Core;
    using Projection.Transformation;

    public class SalaryDepositedTransformer //: IEventTransformer
    {
        public IEnumerable<IEventCore> Transform<TAggregateRoot>(IEvent<TAggregateRoot> candidateEvent) where TAggregateRoot : class, new()
        {
            var salaryDeposited = candidateEvent as SalaryDeposited;
            if (salaryDeposited != null)
                yield return new PayeSalaryDeposited(salaryDeposited.Value);
        }
    }
}