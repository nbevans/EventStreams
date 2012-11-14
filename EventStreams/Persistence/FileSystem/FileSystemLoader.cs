using System;
using System.Collections.Generic;
using System.IO;

namespace EventStreams.Persistence.FileSystem {
    using Core;
    using Serialization.Events;
    using Streams;

    internal sealed class FileSystemLoader : ILoader {
        private readonly RepositoryHierarchy _repositoryHierarchy;
        private readonly IEventReader _eventReader;

        public FileSystemLoader(RepositoryHierarchy repositoryHierarchy, IEventReader eventReader) {
            if (repositoryHierarchy == null) throw new ArgumentNullException("repositoryHierarchy");
            if (eventReader == null) throw new ArgumentNullException("eventReader");
            _repositoryHierarchy = repositoryHierarchy;
            _eventReader = eventReader;
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            FileStream fs;
            try {
                fs = new FileStream(
                    _repositoryHierarchy.For(identity, false),
                    FileMode.Open, FileAccess.Read, FileShare.None,
                    4096, FileOptions.SequentialScan);

            } catch (FileNotFoundException x) {
                throw new StreamNotFoundPersistenceException(identity, _repositoryHierarchy.RootPath, x);

            } catch (DirectoryNotFoundException x) {
                throw new StreamNotFoundPersistenceException(identity, _repositoryHierarchy.RootPath, x);

            } catch (DriveNotFoundException x) {
                throw new StreamNotFoundPersistenceException(identity, _repositoryHierarchy.RootPath, x);                
            }

            // Note: The FileStream is created separately outside the using{} block because
            //       the C# compiler dislikes using the 'yield return' inside a try..catch block.

            using (fs) {
                using (var esw = new EventStreamReader(fs, _eventReader)) {
                    while (esw.HasNext())
                        yield return esw.Next();
                }
            }
        }
    }
}
