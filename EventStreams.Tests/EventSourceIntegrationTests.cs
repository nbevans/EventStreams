using System;

using NUnit.Framework;

namespace EventStreams {
    using Domain;
    using Persistence.FileSystem;
    using Persistence.Serialization.Events;

    [TestFixture]
    public class EventSourceIntegrationTests {

        [Test]
        public void foo() {
            var es =
                new EventSource(
                    new FileSystemPersistenceStrategy(
                        new RepositoryHierarchy("C:\\EventStreams"),
                        EventReaderWriterPair.Json));

            using (var ba = es.Create<BankAccount>()) {
                ba.Credit(150);
                ba.DepositPayeSalary(1500, "Acme Corp");
                ba.Purchase(20, "Steak");
            }
        }

        [Test]
        public void bar() {
            var es =
                new EventSource(
                    new FileSystemPersistenceStrategy(
                        new RepositoryHierarchy("C:\\EventStreams"),
                        EventReaderWriterPair.Json));

            using (var ba = es.Open<BankAccount>(new Guid("a2d06e1b-a311-45c7-9097-d288d61a8c33"))) {
                ba.Purchase(5, "Broadband");
            }
        }
    }
}
