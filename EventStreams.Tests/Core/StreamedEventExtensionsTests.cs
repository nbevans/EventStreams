using System;
using EventStreams.Domain.Events.BankAccount;
using NUnit.Framework;

namespace EventStreams.Core {
    [TestFixture]
    public class StreamedEventExtensionsTests {

        private IStreamedEvent _streamedEvent;

        [TestFixtureSetUp]
        public void Given() {
            _streamedEvent = new StreamedEvent(new Credited(0));
        }

        [Test]
        public void When_args_retrieved_then_a_credited_typed_object_is_returned() {
            Assert.That(_streamedEvent.Args<Credited>(), Is.Not.Null);
        }

        [Test]
        public void When_args_retrieved_with_invalid_type_then_it_will_throw()
        {
            Assert.Throws<InvalidOperationException>(() => _streamedEvent.Args<Debited>());
        }

        [Test]
        public void When_args_or_null_retrieved_with_invalid_type_then_null_is_returned()
        {
            Assert.That(_streamedEvent.ArgsOrNull<Debited>(), Is.Null);
        }
    }
}
