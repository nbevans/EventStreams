﻿using System;
using System.Collections.Generic;
using System.Linq;

using EventStreams.Core;
using EventStreams.Domain;
using EventStreams.Domain.Events.BankAccount;

using NUnit.Framework;

namespace EventStreams.Projection.Transformation {
    [TestFixture]
    public class EventSequenceTransformerTests {

        private readonly Projector _projector = new Projector();

        private readonly StreamedEvent[] _events100 = new[] {
            new PayeSalaryDeposited(100, "Acme Corp").ToStreamedEvent(),
            new SalaryDeposited(50).ToStreamedEvent(),
            new MadePurchase(5, "Cheese").ToStreamedEvent(),
            new MadePurchase(45, "Wine").ToStreamedEvent()
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
            Assert.That(results.ElementAt(0).Args<PayeSalaryDeposited>().Value == 100);
            Assert.That(results.ElementAt(1).Args<PayeSalaryDeposited>().Value == 12.5m);
            Assert.That(results.ElementAt(2).Args<PayeSalaryDeposited>().Value == 12.5m);
            Assert.That(results.ElementAt(3).Args<PayeSalaryDeposited>().Value == 12.5m);
            Assert.That(results.ElementAt(4).Args<PayeSalaryDeposited>().Value == 12.5m);
            Assert.That(results.ElementAt(5).Args<MadePurchase>().Value == 5);
            Assert.That(results.ElementAt(6).Args<MadePurchase>().Value == 45);
        }

        private class SalaryDepositedToFourSplit : IEventTransformer {

            public DateTime Chronology {
                get { return new DateTime(2012, 7, 29); }
            }

            public IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IStreamedEvent candidateEvent) where TAggregateRoot : class, new() {
                var tmp = candidateEvent.Arguments as SalaryDeposited;
                if (tmp != null) {
                    var split = tmp.Value / 4;
                    yield return new SalaryDeposited(split).ToStreamedEvent();
                    yield return new SalaryDeposited(split).ToStreamedEvent();
                    yield return new SalaryDeposited(split).ToStreamedEvent();
                    yield return new SalaryDeposited(split).ToStreamedEvent();
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
                var tmp = candidateEvent.Arguments as SalaryDeposited;
                if (tmp != null) {
                    yield return new PayeSalaryDeposited(tmp.Value, "Unknown").ToStreamedEvent();
                    yield break;
                }

                yield return candidateEvent;
            }
        }
    }
}
