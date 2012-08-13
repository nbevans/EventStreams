using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Snapshots {
    public interface ISerializer {
        void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph);
        TAggregateRoot Deserialize<TAggregateRoot>(Stream stream);
    }
}
