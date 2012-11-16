using System;

using NUnit.Framework;

namespace EventStreams.Projection {
    using Core.Domain;
    using Domain;

    [TestFixture]
    public class ObjectActivatorCacheTests {

        [Test]
        public void Given_a_bank_account_activator_when_activated_with_valid_guid_then_the_created_bank_account_has_that_guid_as_its_identity() {
            var activator = new ObjectActivatorCache<BankAccount>().Activator();
            var arId = new Guid("694B715F-5C81-485C-A0B9-C8156E06F188");
            var ar = activator(arId);

            Assert.That(((IAggregateRoot)ar).Identity == arId);
        }

        [Test]
        public void Given_a_bank_account_activator_when_activated_with_empty_guid_then_the_created_bank_account_will_have_empty_guid_as_its_identity() {
            var activator = new ObjectActivatorCache<BankAccount>().Activator();
            var arId = Guid.Empty;
            var ar = activator(arId);

            Assert.That(((IAggregateRoot)ar).Identity == arId);
        }
    }
}
