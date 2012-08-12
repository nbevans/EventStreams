using System;
using System.IO;

namespace EventStreams.Persistence.Serialization {
    public interface ISerializer {
        void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph);
        TAggregateRoot Deserialize<TAggregateRoot>(Stream stream);
    }
}
