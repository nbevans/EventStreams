using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;

namespace EventStreams.Persistence.Riak.ClusterTools {
    internal enum Health {
        Unknown,
        Okay,
        Degraded
    }

    internal static class RingStatus {
        public static Health Test(string nodeName, out IEnumerable<string> unreachableNodes) {
            var output = Plink.Execute("ring-status.sh", nodeName);

            var match = new Regex("The following nodes are unreachable: \\[(?<UnreachableNodes>.+)\\]",
                                  RegexOptions.Multiline | RegexOptions.ExplicitCapture)
                .Match(output);

            Group tmp;
            if (match.Success && (tmp = match.Groups["UnreachableNodes"]).Success) {
                unreachableNodes = tmp.Value.Split(',').Select(v => v.Substring(1, v.Length - 2));
                return Health.Degraded;
            }

            unreachableNodes = Enumerable.Empty<string>();
            return Health.Okay;
        }

        public static Health Test(string nodeName) {
            IEnumerable<string> tmp;
            return Test(nodeName, out tmp);
        }

        public static void Wait(string nodeName, Health expected = Health.Okay, int seconds = 60) {
            IEnumerable<string> tmp;
            Wait(nodeName, expected, seconds, out tmp);
        }

        private static void Wait(string nodeName, Health expected, int seconds, out IEnumerable<string> unreachableNodes) {
            unreachableNodes = null;

            var health = Health.Unknown;
            var limit = DateTime.UtcNow.AddSeconds(seconds);
            while (DateTime.UtcNow < limit && (health = Test(nodeName, out unreachableNodes)) != expected)
                Thread.Sleep(5000);

            if (health != expected)
                Assert.Fail(
                    "Timed out after {0:N0} seconds whilst waiting for cluster status to be {1}.",
                    seconds,
                    expected);
        }

        public static void AssertOkay(string nodeName, int seconds = 60) {
            Wait(nodeName, Health.Okay, seconds);
        }

        public static void AssertDegraded(string nodeName, int numberOfUnreachableNodes = 2, int seconds = 60) {
            IEnumerable<string> tmp;
            Wait(nodeName, Health.Degraded, seconds, out tmp);
            
            // ReSharper disable PossibleMultipleEnumeration
            if (tmp != null && tmp.Count() != numberOfUnreachableNodes)
                Assert.Fail(
                    "The cluster is not degraded to the expected extent. There should be {0:N0} unreachable nodes, but there is actually {1:N0}.",
                    numberOfUnreachableNodes, tmp.Count());
            // ReSharper restore PossibleMultipleEnumeration
        }
    }
}
