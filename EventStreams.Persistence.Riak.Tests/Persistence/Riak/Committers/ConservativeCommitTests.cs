using System;
using System.Collections.Generic;
using System.Linq;

using CorrugatedIron.Util;
using NUnit.Framework;

namespace EventStreams.Persistence.Riak.Committers {

    [TestFixture]
    public class ConservativeCommitTests {

        private readonly List<string> _firstSet = new[] { "A", "B", "C", "D" }.ToList();
        private readonly List<string> _secondSet = new[] { "E", "F", "G", "H" }.ToList();
        
        [Test]
        public void Given_a_set_of_four_objects_when_stored_and_subsequently_retrieved_then_walking_the_links_from_head_to_tail_matches_the_original_set_of_four() {
            var riakClient = TestingRiakClient.Get();
            var bucket = TestUtils.GetCurrentMethod();

            riakClient.DeleteBucket(bucket, RiakConstants.QuorumOptions.All);

            new ConservativeCommit<string>(riakClient, bucket, _firstSet)
                .Commit();

            riakClient.WalkLinksWhilstAsserting(bucket, PointerKeys.Head, LinkNames.Pointer, PointerKeys.Tail, _firstSet);
        }

        [Test]
        public void Given_both_sets_of_totally_eight_objects_when_stored_in_separate_commits_and_subsequently_retrieved_then_walking_the_links_from_head_to_tail_matches_the_expected_sequence_of_both_sets() {
            var riakClient = TestingRiakClient.Get();
            var bucket = TestUtils.GetCurrentMethod();

            riakClient.DeleteBucket(bucket, RiakConstants.QuorumOptions.All);

            new ConservativeCommit<string>(riakClient, bucket, _firstSet)
                .Commit();

            new ConservativeCommit<string>(riakClient, bucket, _secondSet)
                .Commit();

            riakClient.WalkLinksWhilstAsserting(bucket, PointerKeys.Head, LinkNames.Pointer, PointerKeys.Tail, _firstSet.Concat(_secondSet));
        }
    }
}
