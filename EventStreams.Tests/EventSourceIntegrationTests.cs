using System;

using NUnit.Framework;

namespace EventStreams {
    using Core.Domain;
    using Domain;
    using Persistence.FileSystem;
    using Persistence.Serialization.Events;

    [TestFixture]
    public class EventSourceIntegrationTests {

        [Test]
        public void Create_a_brand_new_stream_and_issue_some_commands() {
            var es =
                new EventSource<BankAccount>(
                    new FileSystemPersistenceStrategy(
                        new RepositoryHierarchy("C:\\EventStreams"),
                        EventReaderWriterPair.Json));

            using (var ba = es.Create()) {
                ba.Credit(150);
                ba.DepositPayeSalary(1500, "Acme Corp");
                ba.Purchase(20, "Steak");

                Assert.That(ba.Balance == 1630);
            }
        }

        [Test]
        public void Open_a_stream_with_a_specific_identifier_and_issue_a_command() {
            var es =
                new EventSource<BankAccount>(
                    new FileSystemPersistenceStrategy(
                        new RepositoryHierarchy("C:\\EventStreams"),
                        EventReaderWriterPair.Json));

            using (var ba = es.Open(new Guid("a2d06e1b-a311-45c7-9097-d288d61a8c33"))) {
                ba.Purchase(5, "Broadband");
            }
        }

        [Test]
        public void Open_or_create_a_stream_with_a_specific_identifier_and_issue_a_command() {
            var es =
                new EventSource<BankAccount>(
                    new FileSystemPersistenceStrategy(
                        new RepositoryHierarchy("C:\\EventStreams"),
                        EventReaderWriterPair.Json));

            using (var ba = es.OpenOrCreate(new Guid("1c8854ec-6399-4464-ade8-3a0e9c914b6c"))) {
                Assert.AreEqual(((IAggregateRoot)ba).Identity, new Guid("1c8854ec-6399-4464-ade8-3a0e9c914b6c"));
                ba.Credit(40);
                ba.Purchase(40, "Sky+");
                Assert.AreEqual(0, ba.Balance);
            }
        }
    }
}
