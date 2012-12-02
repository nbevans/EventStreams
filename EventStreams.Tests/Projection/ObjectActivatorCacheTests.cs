using System;

using NUnit.Framework;

namespace EventStreams.Projection {
    using Domain;

    [TestFixture]
    public class ObjectActivatorCacheTests {

        [Test]
        public void Given_a_bank_account_activator_when_activated_with_valid_guid_then_the_created_bank_account_has_that_guid_as_its_identity() {
            var activator = new ObjectActivatorCache<BankAccount>().Activator();
            var identity = Guid.NewGuid();
            var model = activator(identity);

            Assert.That(model.Identity == identity);
        }

        [Test]
        public void Given_a_bank_account_activator_when_activated_with_empty_guid_then_the_created_bank_account_will_have_empty_guid_as_its_identity() {
            var activator = new ObjectActivatorCache<BankAccount>().Activator();
            var identity = Guid.Empty;
            var model = activator(identity);

            Assert.That(model.Identity == identity);
        }

        [Test]
        public void Given_an_activator_for_a_model_type_when_there_is_no_memento_or_state_parameter_on_its_constructor_then_expect_a_throw() {
            Assert.Throws<InvalidOperationException>(() => new ObjectActivatorCache<BrokenModelType>().Activator());
        }

        [Test]
        public void Given_an_activator_for_a_model_type_with_a_memento_parameter_on_its_constructor_then_expect_no_exception() {
            Assert.DoesNotThrow(() => new ObjectActivatorCache<ValidModelTypeA>().Activator());
        }

        [Test]
        public void Given_an_activator_for_a_model_type_with_a_state_parameter_on_its_constructor_then_expect_no_exception() {
            Assert.DoesNotThrow(() => new ObjectActivatorCache<ValidModelTypeB>().Activator());
        }

        private class BrokenModelType {
            public BrokenModelType() { }
            public BrokenModelType(string somethingSomethingDarkSide) { }
        }

        private class ValidModelTypeA {
            public ValidModelTypeA() { }
            public ValidModelTypeA(string memento) { }
        }

        private class ValidModelTypeB {
            public ValidModelTypeB() { }
            public ValidModelTypeB(string state) { }
        }
    }
}
