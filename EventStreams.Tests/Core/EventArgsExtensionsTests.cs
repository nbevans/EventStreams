using System;

using NUnit.Framework;

namespace EventStreams.Core {
    using EventStreams.Domain.Accounting.Events;

    [TestFixture]
    public class EventArgsExtensionsTests {

        private EventArgs _event;

        [TestFixtureSetUp]
        public void Given() {
            _event = new Credited(0);
        }

        [Test]
        public void When_assumed_correctly_to_be_a_credited_event_then_it_is_casted_fine_and_is_not_null() {
            Assert.That(_event.Assume<Credited>(), Is.Not.Null);
        }

        [Test]
        public void When_assumed_incorrectly_to_be_a_debited_event_then_it_will_throw() {
            Assert.Throws<InvalidOperationException>(() => _event.Assume<Debited>());
        }
    }
}
