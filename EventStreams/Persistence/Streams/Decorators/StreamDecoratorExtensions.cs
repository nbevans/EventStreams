using System;
using System.IO;
using System.Text;

namespace EventStreams.Persistence.Streams.Decorators {
    using Serialization;

    internal static class StreamDecoratorExtensions {
        public static BinaryReader ForBinaryReading(this Stream stream) {
            return new BinaryReader(stream.PreventClosure(), Encoding.UTF8);
        }

        public static BinaryWriter ForBinaryWriting(this Stream stream) {
            return new BinaryWriter(stream.PreventClosure(), Encoding.UTF8);
        }

        public static Stream PreventClosure(this Stream stream) {
            return new NonClosingStream(stream);
        }

        public static Stream VirtualLength(this Stream stream, long length) {
            return new VirtualLengthStream(stream, length);
        }
    }
}
