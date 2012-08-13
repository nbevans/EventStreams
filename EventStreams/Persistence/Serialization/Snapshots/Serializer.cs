using System;
using System.IO;

namespace EventStreams.Persistence.Serialization.Snapshots {
    public class Serializer : ISerializer {
        public void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph) {
            ProtoBuf.Serializer.Serialize(stream, graph);
        }

        public TAggregateRoot Deserialize<TAggregateRoot>(Stream stream) {
            return ProtoBuf.Serializer.Deserialize<TAggregateRoot>(stream);
        }
    }
}
