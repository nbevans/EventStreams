using System;
using System.IO;

namespace EventStreams.Persistence.FileSystem {
    using Core.Domain;

    public sealed class RepositoryHierarchy {

        public string RootPath { get; private set; }

        public RepositoryHierarchy(string rootPath) {
            RootPath = rootPath;
        }

        public string For(Guid identity) {
            return For(identity, false);
        }

        public string For(Guid identity, bool initializePath) {
            var path = GetPath(identity);
            Directory.CreateDirectory(path);
            return Path.Combine(RootPath, path, identity + ".e");
        }

        private string GetPath(Guid identity) {
            var bytes = identity.ToByteArray();
            var a = bytes[3].ToHex();
            var b = bytes[2].ToHex();
            return Path.Combine(RootPath, a, b);
        }
    }
}
