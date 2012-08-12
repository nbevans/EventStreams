using System;
using System.IO;

namespace EventStreams.Persistence.Serialization {
    using ProtoBufSerializer = ProtoBuf.Serializer;

    public class Serializer : ISerializer {
        public void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph) {
            ProtoBufSerializer.Serialize(stream, graph);
        }

        public TAggregateRoot Deserialize<TAggregateRoot>(Stream stream) {
            return ProtoBufSerializer.Deserialize<TAggregateRoot>(stream);
        }
    }
}
