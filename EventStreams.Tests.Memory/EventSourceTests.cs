using System;

using NUnit.Framework;

namespace EventStreams {
    using Core.Domain;
    using Domain;
    using Persistence.FileSystem;
    using Persistence.Serialization.Events;

    [TestFixture]
    public class EventSourceTests {

        [Test]
        public void Given_an_event_source_when_a_object_is_retrieved_and_no_strong_reference_is_held_then_garbage_collection_of_retrieved_object_is_free_to_occur() {
            var es =
                new EventSource(
                    new FileSystemPersistenceStrategy(
                        new RepositoryHierarchy("C:\\EventStreams"),
                        EventReaderWriterPair.Json));

            var finalized = false;
            var ba = es.OpenOrCreate<BankAccountWithGcNotify>(new Guid("a2d06e1b-a311-45c7-9097-d288d61a8c33"));
            ba.FinalizeCallback = () => finalized = true;
            ba = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.That(finalized);  
        }

        private class BankAccountWithGcNotify : BankAccount {

            public Action FinalizeCallback { get; set; }

            public BankAccountWithGcNotify()
                : base(null) { }

            public BankAccountWithGcNotify(Memento<BankAccountState> memento)
                : base(memento) { }

            ~BankAccountWithGcNotify() {
                FinalizeCallback();
            }
        }
    }
}
