using System;
using System.Collections.Generic;
using System.IO;

using EventStreams.Persistence.Serialization.Events;

namespace EventStreams.Persistence {
    using Core;
    using Core.Domain;

    public class FileSystemPersistEvents : IPersistEvents {

        private readonly IEventWriter _eventWriter;

        public string RootPath { get; set; }

        public FileSystemPersistEvents(IEventWriter eventWriter) {
            if (eventWriter == null) throw new ArgumentNullException("eventWriter");
            _eventWriter = eventWriter;

            RootPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void Persist(IAggregateRoot aggregateRoot, IEnumerable<IStreamedEvent> eventsToAppend) {
            using (var fs = new FileStream(GetFileName(aggregateRoot), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.SequentialScan)) {
                fs.Position = fs.Length;

                using (var esw = new EventStreamWriter(fs, _eventWriter))
                    esw.Write(eventsToAppend);
            }
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            throw new NotSupportedException();
        }

        public virtual string GetFileName(IAggregateRoot aggregateRoot) {
            return Path.Combine(RootPath, aggregateRoot.Identity + ".events");
        }
    }
}
