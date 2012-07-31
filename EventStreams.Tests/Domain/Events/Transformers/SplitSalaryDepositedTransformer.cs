using System;
using System.Collections.Generic;

using EventStreams.Domain.Events.BankAccount;

namespace EventStreams.Domain.Events.Transformers {
    using Core;
    using Projection.Transformation;

    public class SplitSalaryDepositedTransformer : IEventTransformer {

        public DateTime Chronology {
            get { return new DateTime(2012, 7, 29); }
        }

        public IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IStreamedEvent candidateEvent) where TAggregateRoot : class, new() {
            var tmp = candidateEvent as SalaryDeposited;
            if (tmp != null) {
                var split = tmp.Value / 4;
                yield return new PayeSalaryDeposited(split, "Unknown - Part 1");
                yield return new PayeSalaryDeposited(split, "Unknown - Part 2");
                yield return new PayeSalaryDeposited(split, "Unknown - Part 3");
                yield return new PayeSalaryDeposited(split, "Unknown - Part 4");
            }
        }
    }
}