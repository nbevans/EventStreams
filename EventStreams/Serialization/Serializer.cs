using System;
using System.IO;

namespace EventStreams.Serialization
{
    public class Serializer
    {
        public void Serialize<TAggregateRoot>(TAggregateRoot graph) where TAggregateRoot : class, new()
        {
            using (var fs = new FileStream(@"test.txt", FileMode.Create, FileAccess.Write, FileShare.None, 1024, false))
                ProtoBuf.Serializer.Serialize(fs, graph);
        }

        public TAggregateRoot Deserialize<TAggregateRoot>() where TAggregateRoot : class, new() {
            using (var fs = new FileStream(@"test.txt", FileMode.Open, FileAccess.Read, FileShare.None, 1024, false))
                return ProtoBuf.Serializer.Deserialize<TAggregateRoot>(fs);
        }
    }
}
