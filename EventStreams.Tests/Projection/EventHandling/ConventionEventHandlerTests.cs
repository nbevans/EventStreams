using System;

using NUnit.Framework;

namespace EventStreams.Projection.EventHandling {
    using Domain.Accounting;
    using Domain.Accounting.Events;

    [TestFixture]
    public class ConventionEventHandlerTests {

        [Test]
        public void Given_a_clean_bank_account_when_credited_with_100_then_balance_is_100() {
            var model = new BankAccount();
            var handler = new ConventionEventHandler(model);

            handler.OnNext(new Credited(100));

            Assert.That(model.Balance == 100);
        }

        [Test]
        public void Given_a_bank_account_when_notified_of_an_event_it_does_not_expect_nor_support_then_expect_a_throw() {
            var model = new BankAccount();
            var handler = new ConventionEventHandler(model);

            Assert.Throws<InvalidOperationException>(() => handler.OnNext(new InvalidEventArgs()));
        }

        private sealed class InvalidEventArgs : EventArgs { }
    }
}
