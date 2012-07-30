using System;
using System.IO;

namespace EventStreams.Serialization
{
    public class Serializer
    {
        public void Serialize<TAggregateRoot>(TAggregateRoot graph) where TAggregateRoot : class, new()
        {
            using (var fs = new FileStream(@"test.txt", FileMode.Create, FileAccess.Write, FileShare.None, 1024, false))
            {
                ProtoBuf.Serializer.Serialize(fs, graph);
                fs.Flush();
            }
        }
    }
}
