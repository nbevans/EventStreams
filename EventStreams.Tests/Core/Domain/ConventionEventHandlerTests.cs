using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EventStreams.Domain;
using EventStreams.Domain.Events.BankAccount;

using NUnit.Framework;

namespace EventStreams.Core.Domain {
    [TestFixture]
    public class ConventionEventHandlerTests {

        [Test]
        public void Given_a_clean_bank_account_when_credited_with_100_then_balance_is_100() {
            var ar = new BankAccount();
            var handler = new ConventionEventHandler<BankAccount>(ar);

            handler.OnNext(new Credited(100));

            Assert.That(ar.Balance == 100);
        }

        [Test]
        public void Given_a_bank_account_when_it_has_completed_handling_events_then_further_attempts_handle_events_will_throw() {
            var ar = new BankAccount();
            var handler = new ConventionEventHandler<BankAccount>(ar);

            handler.OnCompleted();

            Assert.Throws<InvalidOperationException>(() => handler.OnNext(new Credited(1)));
            Assert.Throws<InvalidOperationException>(() => handler.OnCompleted());
            Assert.Throws<InvalidOperationException>(() => handler.OnError(new Exception()));
        }

        [Test]
        public void Given_a_bank_account_when_notified_of_an_event_it_does_not_expect_nor_support_then_expect_a_throw() {
            var ar = new BankAccount();
            var handler = new ConventionEventHandler<BankAccount>(ar);

            Assert.Throws<InvalidOperationException>(() => handler.OnNext(new InvalidEventArgs()));
        }

        private sealed class InvalidEventArgs : EventArgs { }
    }
}
