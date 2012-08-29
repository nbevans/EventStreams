using System;
using System.IO;

namespace EventStreams.Persistence {
    internal static class StreamExtensions {
        public static string ReadStartToEnd(this Stream stream) {
            stream.Position = 0;
            using (var sr = new StreamReader(stream))
                return sr.ReadToEnd();
        }
    }
}
