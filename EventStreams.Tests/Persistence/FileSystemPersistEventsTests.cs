using System;
using System.IO;

using Moq;
using NUnit.Framework;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;
    using Serialization.Events;
    using Domain.Events.BankAccount;

    [TestFixture]
    public class FileSystemPersistEventsTests {

        private readonly IStreamedEvent[] _firstEvents = new[] {
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("20F65C10-D7DE-43B0-A527-4CCC43496BFE") && f.Timestamp == DateTime.MinValue && f.Arguments == new PayeSalaryDeposited(100, "Acme Corp")),
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("685F85DA-07AC-4EB3-B7F5-F52BCD543E84") && f.Timestamp == DateTime.MinValue && f.Arguments == new MadePurchase(45, "Wine"))
        };

        private readonly IStreamedEvent[] _secondEvents = new[] {
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("1012E630-8325-47D4-9393-DCD7F5940E09") && f.Timestamp == DateTime.MinValue && f.Arguments == new PayeSalaryDeposited(150, "Acme Corp")),
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("E19772BA-6DAE-408E-9F09-8561889C8154") && f.Timestamp == DateTime.MinValue && f.Arguments == new MadePurchase(25, "Cheese"))
        };

        [Test]
        public void Given_first_set_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);
            var fspe = new FileSystemPersistEvents(new NullEventWriter());
            {
                var filename = fspe.GetFileName(ar);

                File.Delete(filename);
                fspe.Persist(ar, _firstEvents);

                var content = File.ReadAllText(filename);

                Assert.That(content, Is.EqualTo(Strings.firstEventsStream));
            }
        }

        [Test]
        public void Given_first_set_and_second_set_when_written_to_disk_individually_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);
            var fspe = new FileSystemPersistEvents(new NullEventWriter());
            {
                var filename = fspe.GetFileName(ar);

                File.Delete(filename);
                fspe.Persist(ar, _firstEvents);
                fspe.Persist(ar, _secondEvents);

                var content = File.ReadAllText(filename);

                Assert.That(content, Is.EqualTo(Strings.firstAndSecondEventStreams));
            }
        }
    }
}
