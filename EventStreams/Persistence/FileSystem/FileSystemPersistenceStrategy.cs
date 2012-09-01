using System;
using System.Collections.Generic;

namespace EventStreams.Persistence.FileSystem {
    using Core;
    using Core.Domain;
    using Serialization.Events;

    public class FileSystemPersistenceStrategy : IPersistenceStrategy {
        private readonly FileSystemLoader _loader;
        private readonly FileSystemStorer _storer;

        public FileSystemPersistenceStrategy(RepositoryHierarchy repositoryHierarchy, IEventReaderWriterPair eventReaderWriterPair) {
            if (repositoryHierarchy == null) throw new ArgumentNullException("repositoryHierarchy");
            if (eventReaderWriterPair == null) throw new ArgumentNullException("eventReaderWriterPair");

            _storer = new FileSystemStorer(repositoryHierarchy, eventReaderWriterPair.Writer);
            _loader = new FileSystemLoader(repositoryHierarchy, eventReaderWriterPair.Reader);
        }

        public void Store(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> eventsToAppend) {
            _storer.Store(aggregateRoot, eventsToAppend);
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            return _loader.Load(identity);
        }
    }
}
