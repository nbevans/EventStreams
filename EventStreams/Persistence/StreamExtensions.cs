using System;
using System.IO;
using System.Text;

namespace EventStreams.Persistence {
    internal static class StreamExtensions {
        public static BinaryReader ForBinaryReading(this Stream stream) {
            return new BinaryReader(new NonClosingStream(stream), Encoding.UTF8);
        }

        public static BinaryWriter ForBinaryWriting(this Stream stream) {
            return new BinaryWriter(new NonClosingStream(stream), Encoding.UTF8);
        }
    }
}
