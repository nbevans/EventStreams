using System;
using System.IO;

using NUnit.Framework;

namespace EventStreams.Persistence.FileSystem {
    using Resources;
    using Serialization.Events;
    using Streams;

    [TestFixture]
    public class FileSystemPersistenceStrategyTests {

        private readonly RepositoryHierarchy _repositoryPath =
            new RepositoryHierarchy(AppDomain.CurrentDomain.BaseDirectory);

        [Test]
        public void Given_first_set_when_written_to_disk_and_when_read_back_in_then_content_is_as_expected() {
            var identity = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");

            var filename = _repositoryPath.For(identity);
            File.Delete(filename);

            var fspe = new FileSystemPersistenceStrategy(_repositoryPath, EventReaderWriterPair.Null);
            fspe.Store(identity, MockEventStreams.First);

            Assert.AreEqual(File.ReadAllText(filename), ResourceProvider.Get("First.e"));
        }

        [Test]
        public void Given_first_set_and_second_set_when_written_to_disk_individually_and_when_read_back_in_then_content_is_as_expected() {
            var identity = new Guid("E34900D6-6C63-4066-988F-DCEC25B482FA");

            var filename = _repositoryPath.For(identity);
            File.Delete(filename);

            var fspe = new FileSystemPersistenceStrategy(_repositoryPath, EventReaderWriterPair.Null);
            fspe.Store(identity, MockEventStreams.First);
            fspe.Store(identity, MockEventStreams.Second);

            Assert.AreEqual(File.ReadAllText(filename), ResourceProvider.Get("First_and_second.e"));
        }
    }
}
