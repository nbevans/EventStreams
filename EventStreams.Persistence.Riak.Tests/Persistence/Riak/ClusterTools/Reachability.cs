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

    internal class Reachability {
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

        public static void AssertOkay(string nodeName) {
            var health = Health.Unknown;
            for (var i = 0; i < 3 && (health = Test(nodeName)) != Health.Okay; i++)
                Thread.Sleep(5000);

            if (health != Health.Okay)
                Assert.Fail("The cluster health is not okay.");
        }

        public static void AssertDegraded(string nodeName, int numberOfUnreachableNodes = 2) {
            var health = Health.Unknown;
            IEnumerable<string> tmp = null;
            for (var i = 0; i < 3 && (health = Test(nodeName, out tmp)) != Health.Degraded; i++)
                Thread.Sleep(5000);
            
            if (health != Health.Degraded)
                Assert.Fail("The cluster health is not degraded.");

            // ReSharper disable PossibleMultipleEnumeration
            if (tmp != null && tmp.Count() != numberOfUnreachableNodes)
                Assert.Fail(
                    "The cluster is not degraded to the expected extent. There should be {0:N0} unreachable nodes, but there is actually {1:N0}.",
                    numberOfUnreachableNodes, tmp.Count());
            // ReSharper restore PossibleMultipleEnumeration
        }
    }
}
