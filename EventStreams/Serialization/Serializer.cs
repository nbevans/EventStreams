using System;
using System.IO;

namespace EventStreams.Serialization {
    using ProtoBufSerializer = ProtoBuf.Serializer;

    public class Serializer : ISerializer {
        public void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph) where TAggregateRoot : class, new() {
            ProtoBufSerializer.Serialize(stream, graph);
        }

        public TAggregateRoot Deserialize<TAggregateRoot>(Stream stream) where TAggregateRoot : class, new() {
            return ProtoBufSerializer.Deserialize<TAggregateRoot>(stream);
        }
    }
}
