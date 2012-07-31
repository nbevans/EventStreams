using System;
using System.IO;

namespace EventStreams.Serialization {
    public interface ISerializer {
        void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph) where TAggregateRoot : class, new();
        TAggregateRoot Deserialize<TAggregateRoot>(Stream stream) where TAggregateRoot : class, new();
    }
}
