using System;
using System.IO;
using Moq;
using NUnit.Framework;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;
    using Domain.Events.BankAccount;

    [TestFixture]
    public class FileSystemPersistEventsTests {

        private readonly IStreamedEvent[] _events = new[] {
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("20F65C10-D7DE-43B0-A527-4CCC43496BFE") && f.Timestamp == DateTime.MinValue && f.Arguments == new PayeSalaryDeposited(100, "Acme Corp")),
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("685F85DA-07AC-4EB3-B7F5-F52BCD543E84") && f.Timestamp == DateTime.MinValue && f.Arguments == new MadePurchase(45, "Wine"))
        };

        [Test]
        public void Given_two_events_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);
            var fspe = new FileSystemPersistEvents();

            File.Delete(fspe.GetFileName(ar));
            fspe.Persist(ar, _events);
            Assert.That(
                File.ReadAllText(fspe.GetFileName(ar)),
                Is.EqualTo(Strings.FileSystemPersistEventsTests___Given_events100_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected));
        }
    }
}
