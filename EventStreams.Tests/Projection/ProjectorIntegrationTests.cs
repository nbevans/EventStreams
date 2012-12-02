using System;
using System.Linq;
using EventStreams.Core;
using EventStreams.Domain;
using EventStreams.Domain.Events.BankAccount;

using NUnit.Framework;

namespace EventStreams.Projection {
    using Core.Domain;

    [TestFixture]
    public class ProjectorIntegrationTests {

        private readonly Projector _projector = new Projector();

        private readonly Func<BankAccount, EventHandler<BankAccount>> _defaultEventHandlerFactory =
            model => new ConventionEventHandler<BankAccount>(model, EventHandlerBehavior.Lossless);

        private readonly StreamedEvent[] _events100 = new[] {
            new PayeSalaryDeposited(100, "Acme Corp").ToStreamedEvent(),
            new PayeSalaryDeposited(50, "Acme Corp").ToStreamedEvent(),
            new MadePurchase(5, "Cheese").ToStreamedEvent(),
            new MadePurchase(45, "Wine").ToStreamedEvent()
        };

        private readonly StreamedEvent[] _events240 = new[] {
            new PayeSalaryDeposited(200, "Acme Corp").ToStreamedEvent(),
            new PayeSalaryDeposited(100, "Acme Corp").ToStreamedEvent(),
            new MadePurchase(10, "Cheese").ToStreamedEvent(),
            new MadePurchase(50, "Wine").ToStreamedEvent()
        };

        private readonly StreamedEvent[] _events725 = new[] {
            new PayeSalaryDeposited(200, "Acme Corp").ToStreamedEvent(),
            new PayeSalaryDeposited(100, "Acme Corp").ToStreamedEvent(),
            new MadePurchase(10, "Cheese").ToStreamedEvent(),
            new MadePurchase(50, "Wine").ToStreamedEvent(),
            new PayeSalaryDeposited(500, "Acme Corp").ToStreamedEvent(),
            new MadePurchase(15, "Beer").ToStreamedEvent()
        };

        [Test]
        public void Given_events_100_sequence_then_balance_equals_100() {
            var obj1 = _projector.Project(_events100, _defaultEventHandlerFactory);
            Assert.That(obj1.Balance == 100);
        }

        [Test]
        public void Given_all_events_sequences_then_balance_equals_100_240_and_725_respectively() {
            var obj1 = _projector.Project(_events100, _defaultEventHandlerFactory);
            var obj2 = _projector.Project(_events240, _defaultEventHandlerFactory);
            var obj3 = _projector.Project(_events725, _defaultEventHandlerFactory);
            Assert.That(obj1.Balance == 100);
            Assert.That(obj2.Balance == 240);
            Assert.That(obj3.Balance == 725);
        }

        [Test]
        public void Given_a_null_event_sequence_then_a_clean_aggregate_root_is_returned() {
            var obj = _projector.Project(null, _defaultEventHandlerFactory);
            Assert.That(obj.Balance == 0);
        }

        [Test]
        public void Given_an_empty_event_sequence_then_a_clean_aggregate_root_is_returned() {
            var obj = _projector.Project(Enumerable.Empty<IStreamedEvent>(), _defaultEventHandlerFactory);
            Assert.That(obj.Balance == 0);
        }
    }
}
