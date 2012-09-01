using System;
using System.IO;

namespace EventStreams.Persistence.FileSystem {
    using Core.Domain;

    public sealed class RepositoryHierarchy {

        public string RootPath { get; private set; }

        public RepositoryHierarchy(string rootPath) {
            RootPath = rootPath;
        }

        public string For(IAggregateRoot aggregateRoot) {
            return For(aggregateRoot, false);
        }

        public string For(IAggregateRoot aggregateRoot, bool initializePath) {
            if (aggregateRoot == null) throw new ArgumentNullException("aggregateRoot");
            return For(aggregateRoot.Identity, initializePath);
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
            return Path.Combine(RootPath, ".es", a, b);
        }
    }
}
