using System;

using NUnit.Framework;

namespace EventStreams {
    using Domain.Accounting;
    using Persistence;

    [TestFixture]
    public class EventSourceTests {

        [Test]
        public void Given_an_event_source_when_a_object_is_retrieved_and_no_strong_reference_is_held_then_garbage_collection_of_retrieved_object_is_free_to_occur() {
            var es =
                new EventSource(new NullPersistenceStrategy());

            var finalized = false;
            var ba = es.OpenOrCreate<BankAccountWithGcNotify>(new Guid("a2d06e1b-a311-45c7-9097-d288d61a8c33"));
            ba.FinalizeCallback = () => finalized = true;
// ReSharper disable RedundantAssignment
            ba = null;
// ReSharper restore RedundantAssignment

            GC.Collect();
            GC.WaitForPendingFinalizers();

            // Note: This test will fail in NCrunch with default settings. You must turn off the "Analyse line execution times"
            //       function for this project only under the NCrunch -> Configuration screen.
            Assert.That(finalized);  
        }

// ReSharper disable ClassNeverInstantiated.Local
        private class BankAccountWithGcNotify : BankAccount {

            public Action FinalizeCallback { get; set; }

// ReSharper disable UnusedMember.Local
            public BankAccountWithGcNotify()
                : base(null) { }

            public BankAccountWithGcNotify(State memento)
// ReSharper restore UnusedMember.Local
                : base(memento) { }

            ~BankAccountWithGcNotify() {
                FinalizeCallback();
            }
        }
// ReSharper restore ClassNeverInstantiated.Local
    }
}
