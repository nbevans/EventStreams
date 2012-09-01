using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EventStreams.Domain;
using EventStreams.Persistence.FileSystem;
using EventStreams.Persistence.Serialization.Events;

using NUnit.Framework;

namespace EventStreams {

    [TestFixture]
    public class EventSourceIntegrationTests {

        [Test]
        public void foo() {
            var es =
                new EventSource(
                    new FileSystemPersistenceStrategy(
                        new RepositoryHierarchy("T:\\EventStreams"),
                        EventReaderWriterPair.Json));

            using (var ba = es.Create<BankAccount>()) {
                ba.Credit(150);
                ba.DepositPayeSalary(1500, "Acme Corp");
                ba.Purchase(20, "Steak");
            }
        }
    }
}
