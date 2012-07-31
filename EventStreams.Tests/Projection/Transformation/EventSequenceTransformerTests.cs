using System;
using System.Collections.Generic;
using System.Linq;

using EventStreams.Core;
using EventStreams.Domain;
using EventStreams.Domain.Events.BankAccount;
using EventStreams.Domain.Events.Transformers;

using NUnit.Framework;

namespace EventStreams.Projection.Transformation {
    [TestFixture]
    public class EventSequenceTransformerTests {

        private readonly Projector _projector = new Projector();

        private readonly StreamedEventArgs[] _events100 = new StreamedEventArgs[] {
            new PayeSalaryDeposited(100, "Acme Corp"),
            new SalaryDeposited(50),
            new MadePurchase(5, "Cheese"),
            new MadePurchase(45, "Wine")
        };

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            _projector.Cache<BankAccount>();
        }

        [Test]
        public void Given_two_cascading_transformers_the_sequencer_outputs_expected_types_and_order() {
            var transformer =
                (EventSequenceTransformer)
                new EventSequenceTransformer()
                    .Bind<SalaryDepositedToFourSplit>()
                    .Bind<SalaryDepositedToPayeSalaryDeposited>();

            var results = transformer.Transform<BankAccount>(_events100).ToList();

            Assert.That(results != null);
            Assert.That(results.Count() == 7);
            Assert.That(((PayeSalaryDeposited)results.ElementAt(0)).Value == 100);
            Assert.That(((PayeSalaryDeposited)results.ElementAt(1)).Value == 12.5m);
            Assert.That(((PayeSalaryDeposited)results.ElementAt(2)).Value == 12.5m);
            Assert.That(((PayeSalaryDeposited)results.ElementAt(3)).Value == 12.5m);
            Assert.That(((PayeSalaryDeposited)results.ElementAt(4)).Value == 12.5m);
            Assert.That(((MadePurchase)results.ElementAt(5)).Value == 5);
            Assert.That(((MadePurchase)results.ElementAt(6)).Value == 45);
        }

        private class SalaryDepositedToFourSplit : IEventTransformer {

            public DateTime Chronology {
                get { return new DateTime(2012, 7, 29); }
            }

            public IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IStreamedEvent candidateEvent) where TAggregateRoot : class, new() {
                var tmp = candidateEvent as SalaryDeposited;
                if (tmp != null) {
                    var split = tmp.Value / 4;
                    yield return new SalaryDeposited(split);
                    yield return new SalaryDeposited(split);
                    yield return new SalaryDeposited(split);
                    yield return new SalaryDeposited(split);
                    yield break;
                }

                yield return candidateEvent;
            }
        }

        private class SalaryDepositedToPayeSalaryDeposited : IEventTransformer {

            public DateTime Chronology {
                get { return new DateTime(2012, 7, 30); }
            }

            public IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IStreamedEvent candidateEvent) where TAggregateRoot : class, new() {
                var tmp = candidateEvent as SalaryDeposited;
                if (tmp != null) {
                    yield return new PayeSalaryDeposited(tmp.Value, "Unknown");
                    yield break;
                }

                yield return candidateEvent;
            }
        }
    }
}
