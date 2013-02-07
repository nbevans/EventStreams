using System;
using System.Linq;

using CorrugatedIron.Util;
using NUnit.Framework;

namespace EventStreams.Persistence.Riak.Committers {

    [TestFixture]
    public class ConservativeCommitTests {

        [Test]
        public void Given_a_set_of_four_objects_when_stored_and_subsequently_retrieved_from_that_store_then_walking_the_links_from_head_to_tail_matches_the_original_set_of_four() {
            var riakClient = TestingRiakClient.Get();
            var bucket = TestUtils.GetCurrentMethod();
            var objects = new[] { "A", "B", "C", "D" }.ToList();

            riakClient.DeleteBucket(bucket, RiakConstants.QuorumOptions.All);

            new ConservativeCommit<string>(riakClient, bucket, objects, v => v)
                .Commit();

            riakClient.WalkLinksWhilstAsserting(bucket, PointerKeys.Head, LinkNames.Pointer, PointerKeys.Tail, objects);
        }
    }
}
