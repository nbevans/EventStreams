using System;
using System.IO;

namespace EventStreams.Persistence.FileSystem {
    using Core.Domain;

    public sealed class RepositoryHierarchy {

        public string RootPath { get; private set; }

        public RepositoryHierarchy(string rootPath) {
            RootPath = rootPath;
        }

        public void Initialize(IAggregateRoot aggregateRoot) {
            if (aggregateRoot == null) throw new ArgumentNullException("aggregateRoot");
            Initialize(aggregateRoot.Identity);
        }

        public void Initialize(Guid identity) {
            var path = GetPath(identity);
            Directory.CreateDirectory(path);
        }

        public string For(IAggregateRoot aggregateRoot) {
            if (aggregateRoot == null) throw new ArgumentNullException("aggregateRoot");
            return For(aggregateRoot.Identity);
        }

        public string For(Guid identity) {
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
