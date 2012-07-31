using System;
using System.IO;

namespace EventStreams.Serialization
{
    public class Serializer
    {
        public void Serialize<TAggregateRoot>(Stream stream, TAggregateRoot graph) where TAggregateRoot : class, new()
        {
            //using (var fs = new FileStream(@"test.txt", FileMode.Create, FileAccess.Write, FileShare.None, 1024, false))
            ProtoBuf.Serializer.Serialize(stream, graph);
        }

        public TAggregateRoot Deserialize<TAggregateRoot>(Stream stream) where TAggregateRoot : class, new() {
            //using (var fs = new FileStream(@"test.txt", FileMode.Open, FileAccess.Read, FileShare.None, 1024, false))
            return ProtoBuf.Serializer.Deserialize<TAggregateRoot>(stream);
        }
    }
}
