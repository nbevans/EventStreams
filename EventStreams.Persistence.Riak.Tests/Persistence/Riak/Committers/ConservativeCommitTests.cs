using System;
using System.Linq;

using NUnit.Framework;

namespace EventStreams.Persistence.Riak.Committers {
    using System.Collections.Generic;
    using CorrugatedIron;

    [TestFixture]
    public class ConservativeCommitTests {

        [Test]
        public void foo() {
            var riakClient = TestingRiakClient.Get();
            var bucket = TestUtils.GetCurrentMethod();
            var objects = new[] { "A", "B", "C", "D" }.ToList();

            riakClient.DeleteBucket(bucket);

            new ConservativeCommit<string>(riakClient, bucket, objects, v => v).Commit();

            WalkLinks(riakClient, bucket, PointerKeys.Head, LinkNames.Pointer, PointerKeys.Tail, objects);
        }

        private static void WalkLinks(IRiakClient riakClient, string bucket, string startingKey, string navigatingLink, string endingKey, IEnumerable<string> expectedObjects) {
            var rr = riakClient.Get(bucket, startingKey);
            while (rr.IsSuccess && !rr.Value.Key.Equals(endingKey, StringComparison.Ordinal)) {
                var next = rr.Value.Links.First(l => l.Tag.Equals(navigatingLink, StringComparison.Ordinal)).Key;
                rr = riakClient.Get(bucket, next);
            }
        }
    }
}
