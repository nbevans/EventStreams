using System;
using System.IO;
using System.Reflection;

namespace EventStreams.Persistence.Resources {
    public class ResourceProvider {

        public static string Get(string name) {
            using (var rs = Assembly.GetCallingAssembly().GetManifestResourceStream(typeof(ResourceProvider), name))
                if (rs != null)
                    using (var sr = new StreamReader(rs))
                        return sr.ReadToEnd();
                else
                    throw new InvalidOperationException(
                        string.Format(
                            "The test resource file ({0}) does not exist.",
                            name));
        }

        public static void AppendTo(Stream stream, string name) {
            using (var rs = Assembly.GetCallingAssembly().GetManifestResourceStream(typeof(ResourceProvider), name)) {
                var buffer = new byte[rs.Length];
                rs.Read(buffer, 0, buffer.Length);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        public static void Dump(Stream stream, string path, string name) {
            stream.Position = 0;
            var filename = Path.Combine(path, name);
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            File.WriteAllBytes(filename, buffer);
            stream.Position = 0;
        }
    }
}
