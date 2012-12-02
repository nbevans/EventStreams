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
            Assert.Throws<InvalidOperationException>(() => new ObjectActivatorCache<BrokenModelTypeA>().Activator());
        }

        [Test]
        public void Given_an_activator_for_a_model_type_with_a_memento_parameter_on_its_constructor_then_expect_no_exception() {
            Assert.DoesNotThrow(() => new ObjectActivatorCache<ValidModelTypeA>().Activator());
        }

        [Test]
        public void Given_an_activator_for_a_model_type_with_a_state_parameter_on_its_constructor_then_expect_no_exception() {
            Assert.DoesNotThrow(() => new ObjectActivatorCache<ValidModelTypeB>().Activator());
        }

        [Test]
        public void Given_an_activator_for_a_model_type_when_its_constructor_uses_a_broken_memento_type_then_expect_a_throw() {
            Assert.Throws<InvalidOperationException>(() => new ObjectActivatorCache<BrokenModelTypeB>().Activator());           
        }

        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedParameter.Local
        private class BrokenModelTypeA {
            public BrokenModelTypeA(string foobar) { }
        }

        private class BrokenModelTypeB {
            public BrokenModelTypeB(BrokenModelTypeState memento) { }
        }

        private class ValidModelTypeA {
            public ValidModelTypeA(ValidModelTypeState memento) { }
        }

        private class ValidModelTypeB {
            public ValidModelTypeB(ValidModelTypeState state) { }
        }

        private class BrokenModelTypeState {
            public BrokenModelTypeState(string foobar) { }
        }

        private class ValidModelTypeState {
            public ValidModelTypeState(Guid identity) { }
        }
        // ReSharper restore UnusedParameter.Local
        // ReSharper restore ClassNeverInstantiated.Local
    }
}
