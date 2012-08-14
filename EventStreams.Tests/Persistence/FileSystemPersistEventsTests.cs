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

        private readonly StreamedEvent[] _events100 = new[] {
            new PayeSalaryDeposited(100, "Acme Corp").ToStreamedEvent("20F65C10-D7DE-43B0-A527-4CCC43496BFE"),
            new PayeSalaryDeposited(50, "Acme Corp").ToStreamedEvent("A621BC38-BA90-46C0-A411-02A2936B4952"),
            new MadePurchase(5, "Cheese").ToStreamedEvent("6C5ED58D-1163-48AE-B9B9-0B38FEBBD26C"),
            new MadePurchase(45, "Wine").ToStreamedEvent("685F85DA-07AC-4EB3-B7F5-F52BCD543E84")
        };

        [Test]
        public void Given_events100_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);
            var fspe = new FileSystemPersistEvents();

            File.Delete(fspe.GetFileName(ar));
            fspe.Persist(ar, _events100);
            Assert.That(
                File.ReadAllText(fspe.GetFileName(ar)),
                Is.EqualTo(Strings.FileSystemPersistEventsTests___Given_events100_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected));
        }
    }
}
