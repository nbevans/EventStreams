using System;

using NUnit.Framework;

namespace EventStreams.Core.Domain {
    using EventStreams.Domain.Accounting;
    using EventStreams.Domain.Accounting.Events;

    [TestFixture]
    public class DelegatedEventHandlerTests {

        [Test]
        public void Given_a_clean_bank_account_when_credited_with_100_then_balance_is_100() {
            decimal balance = 0;
            var handler =
                new DelegatedEventHandler<BankAccount>(new BankAccount())
                    .Bind<Credited>(e => balance += e.Assume<Credited>().Value);

            handler.OnNext(new Credited(100));

            Assert.That(balance == 100);
        }

        [Test]
        public void Given_a_bank_account_when_it_has_completed_handling_events_then_further_attempts_handle_events_will_throw() {
            var handler =
                new DelegatedEventHandler<BankAccount>(new BankAccount())
                    .Bind<Credited>(e => { });

            handler.OnCompleted();

            Assert.Throws<InvalidOperationException>(() => handler.OnNext(new Credited(1)));
            Assert.Throws<InvalidOperationException>(() => handler.OnCompleted());
            Assert.Throws<InvalidOperationException>(() => handler.OnError(new Exception()));
        }

        [Test]
        public void Given_a_bank_account_when_notified_of_an_event_it_does_not_expect_nor_support_then_expect_a_throw() {
            var handler =
                new DelegatedEventHandler<BankAccount>(new BankAccount())
                    .Bind<Credited>(e => { });

            Assert.Throws<InvalidOperationException>(() => handler.OnNext(new InvalidEventArgs()));
        }

        private sealed class InvalidEventArgs : EventArgs { }
    }
}
