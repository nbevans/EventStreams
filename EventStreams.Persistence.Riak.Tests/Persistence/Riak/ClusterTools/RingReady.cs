using System;
using System.Threading;

namespace EventStreams.Persistence.Riak.ClusterTools {
    internal class RingReady {
        public static bool Test(string nodeName) {
            var output = Plink.Execute("ring-ready.sh", nodeName);
            return output.IndexOf("TRUE ", 0, StringComparison.OrdinalIgnoreCase) > 0;
        }

        public static void Assert(string nodeName, bool expected) {
            Wait(nodeName, expected);
        }

        public static void Wait(string nodeName, bool expected = true, int seconds = 60) {
            var health = false;
            var limit = DateTime.UtcNow.AddSeconds(seconds);
            while (DateTime.UtcNow < limit && (health = Test(nodeName)) != expected)
                Thread.Sleep(5000);

            if (health != expected)
                NUnit.Framework.Assert.Fail(
                    "Timed out after {0:N0} seconds whilst waiting for cluster ring readiness to be \"{1}\".",
                    seconds, expected);
        }
    }
}
