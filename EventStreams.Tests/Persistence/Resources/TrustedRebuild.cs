using System;
using System.IO;

using NUnit.Framework;

namespace EventStreams.Persistence.Resources {
    using Streams;
    using Serialization.Events;

    [TestFixture]
    [Ignore("Must be run manually.")]
    public class TrustedRebuild {

        private const string ResourcesPath = @"F:\Sandbox (Hg)\EventStreams\EventStreams.Tests\Persistence\Resources\";

        [Test]
        public void First() {
            Write("First.e", esw => esw.Write(MockEventStreams.First));
        }

        [Test]
        public void First_and_second() {
            Write("First_and_second.e", esw => {
                esw.Write(MockEventStreams.First);
                esw.Write(MockEventStreams.Second);
            });
        }

        [Test]
        public void First_with_hash_seed() {
            Write("First_with_hash_seed.e", esw => {
                esw.Write(MockEventStreams.First);
                esw.Write(MockEventStreams.First);
            });
        }

        private static void Write(string name, Action<EventStreamWriter> action) {
            var filename = Path.Combine(ResourcesPath, name);
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            using (var esw = new EventStreamWriter(fs, new NullEventWriter())) {
                action(esw);
            }
        }
    }
}
