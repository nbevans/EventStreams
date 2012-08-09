using System;
using EventStreams.Core;
using EventStreams.Domain;
using EventStreams.Domain.Events.BankAccount;

using NUnit.Framework;

namespace EventStreams.Projection {
    [TestFixture]
    public class ProjectorIntegrationTests {

        private readonly Projector _projector = new Projector();

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
            new MadePurchase(15, "Beer").ToStreamedEvent(),
        };

        [Test]
        public void Given_events_100_sequence_then_balance_equals_100() {
            var obj1 = _projector.Project<BankAccount>(_events100);
            Assert.That(obj1.Balance == 100);
        }

        [Test]
        public void Given_all_events_sequences_then_balance_equals_100_240_and_725_respectively() {
            var obj1 = _projector.Project<BankAccount>(_events100);
            var obj2 = _projector.Project<BankAccount>(_events240);
            var obj3 = _projector.Project<BankAccount>(_events725);
            Assert.That(obj1.Balance == 100);
            Assert.That(obj2.Balance == 240);
            Assert.That(obj3.Balance == 725);
        }
    }
}
