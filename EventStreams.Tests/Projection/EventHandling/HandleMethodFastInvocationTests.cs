using System;

using NUnit.Framework;

namespace EventStreams.Projection.EventHandling {
    using Domain.Accounting;
    using Domain.Accounting.Events;

    [TestFixture]
    public class HandleMethodFastInvocationTests {

        [Test]
        public void Given_a_bank_account_when_credited_with_100_via_direct_handle_method_invocation_then_expect_100_as_a_balance() {
            var ba = new BankAccount();
            var e = new Credited(100);

            Action<object, EventArgs> method;
            if (new HandleMethodFastInvocation(typeof(BankAccount)).TryGetMethod(e, out method))
                method(ba, e);

            Assert.AreEqual(100, ba.Balance);
        }

        [Test]
        public void Given_a_bank_account_when_trying_to_get_an_unsupported_event_type_then_expect_no_return() {
            Action<object, EventArgs> method;
            Assert.IsFalse(new HandleMethodFastInvocation(typeof(BankAccount)).TryGetMethod(new BogusEventArgs(), out method));
        }

        [Test]
        public void Given_a_bank_account_when_trying_to_get_the_ultimate_base_class_type_then_expect_no_return() {
            Action<object, EventArgs> method;
            Assert.IsFalse(new HandleMethodFastInvocation(typeof(BankAccount)).TryGetMethod(new EventArgs(), out method));
        }

        private sealed class BogusEventArgs : EventArgs { }
    }
}
