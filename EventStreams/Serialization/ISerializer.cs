using System;
using System.IO;

namespace EventStreams.Serialization {
    public interface ISerializer {
        void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph);
        TAggregateRoot Deserialize<TAggregateRoot>(Stream stream);
    }
}
