using System;
using System.IO;

using Moq;
using NUnit.Framework;

namespace EventStreams.Persistence.FileSystem {
    using Core.Domain;
    using Resources;
    using Serialization.Events;
    using Streams;

    [TestFixture]
    public class FileSystemPersistenceStrategyTests {

        private readonly RepositoryHierarchy _repositoryPath =
            new RepositoryHierarchy(AppDomain.CurrentDomain.BaseDirectory);

        [Test]
        public void Given_first_set_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);

            var filename = _repositoryPath.For(ar);
            File.Delete(filename);

            var fspe = new FileSystemPersistenceStrategy(_repositoryPath, EventReaderWriterPair.Null);
            fspe.Store(ar, MockEventStreams.First);

            Assert.AreEqual(File.ReadAllText(filename), ResourceProvider.Get("First.e"));
        }

        [Test]
        public void Given_first_set_and_second_set_when_written_to_disk_individually_and_when_read_back_in_then_content_is_as_expected() {
            var arId = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");
            var ar = Mock.Of<IAggregateRoot>(f => f.Identity == arId);

            var filename = _repositoryPath.For(ar);
            File.Delete(filename);

            var fspe = new FileSystemPersistenceStrategy(_repositoryPath, EventReaderWriterPair.Null);
            fspe.Store(ar, MockEventStreams.First);
            fspe.Store(ar, MockEventStreams.Second);

            Assert.AreEqual(File.ReadAllText(filename), ResourceProvider.Get("First_and_second.e"));
        }
    }
}
