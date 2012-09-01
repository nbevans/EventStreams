using System;
using System.Collections.Generic;
using System.IO;

namespace EventStreams.Persistence.FileSystem {
    using Core;
    using Serialization.Events;
    using Streams;

    internal sealed class FileSystemLoader : ILoader {
        private readonly RepositoryHierarchy _repositoryPath;
        private readonly IEventReader _eventReader;

        public FileSystemLoader(RepositoryHierarchy repositoryPath, IEventReader eventReader) {
            if (repositoryPath == null) throw new ArgumentNullException("repositoryPath");
            if (eventReader == null) throw new ArgumentNullException("eventReader");
            _repositoryPath = repositoryPath;
            _eventReader = eventReader;
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            var filename = _repositoryPath.For(identity, false);
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, 4096, FileOptions.SequentialScan)) {
                using (var esw = new EventStreamReader(fs, _eventReader))
                    yield return esw.Next();
            }
        }
    }
}
