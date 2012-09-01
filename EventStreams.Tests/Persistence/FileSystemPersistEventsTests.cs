using System;
using System.IO;

using Moq;
using NUnit.Framework;

namespace EventStreams.Persistence {
    using Streams;
    using Core.Domain;
    using Serialization.Events;
    using Resources;

    [TestFixture]
    public class FileSystemPersistEventsTests {

        [Test]
        public void Given_first_set_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);
            var fspe = new FileSystemPersistEvents(EventReaderWriterPair.Null); {
                var filename = fspe.GetFileName(ar);

                File.Delete(filename);
                fspe.Persist(ar, MockEventStreams.First);

                var actual = File.ReadAllText(filename);
                var expected = ResourceProvider.Get("First.e");

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void Given_first_set_and_second_set_when_written_to_disk_individually_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);
            var fspe = new FileSystemPersistEvents(EventReaderWriterPair.Null); {
                var filename = fspe.GetFileName(ar);

                File.Delete(filename);
                fspe.Persist(ar, MockEventStreams.First);
                fspe.Persist(ar, MockEventStreams.Second);

                var actual = File.ReadAllText(filename);
                var expected = ResourceProvider.Get("First_and_second.e");

                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}
