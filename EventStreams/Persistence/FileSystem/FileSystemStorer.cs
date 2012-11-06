using System;
using System.Collections.Generic;
using System.IO;

namespace EventStreams.Persistence.FileSystem {
    using Core;
    using Core.Domain;
    using Serialization.Events;
    using Streams;

    internal sealed class FileSystemStorer : IStorer {
        private readonly RepositoryHierarchy _repositoryHierarchy;
        private readonly IEventWriter _eventWriter;

        public FileSystemStorer(RepositoryHierarchy repositoryHierarchy, IEventWriter eventWriter) {
            if (repositoryHierarchy == null) throw new ArgumentNullException("repositoryHierarchy");
            if (eventWriter == null) throw new ArgumentNullException("eventWriter");
            _repositoryHierarchy = repositoryHierarchy;
            _eventWriter = eventWriter;
        }

        public void Store(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> eventsToAppend) {
            var filename = _repositoryHierarchy.For(aggregateRoot, true);
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.SequentialScan)) {
                fs.Position = fs.Length;

                using (var esw = new EventStreamWriter(fs, _eventWriter))
                    esw.Write(eventsToAppend);
            }
        }
    }
}
