using System;
using System.Collections.Generic;
using System.Linq;

using CorrugatedIron;
using NUnit.Framework;

namespace EventStreams.Persistence.Riak {

    internal static class RiakClientExtensions {

        public static void WalkLinksWhilstAsserting<T>(this IRiakClient riakClient, string bucket, string startingKey, string navigatingLink, string endingKey, IEnumerable<T> expectedObjects) {
            var rr = riakClient.Get(bucket, startingKey);
            var enumer = expectedObjects.GetEnumerator();
            while (enumer.MoveNext() && rr.IsSuccess && !rr.Value.Key.Equals(endingKey, StringComparison.Ordinal)) {
                var next = rr.Value.Links.Single(l => l.Tag.Equals(navigatingLink, StringComparison.Ordinal)).Key;
                rr = riakClient.Get(bucket, next);

                Assert.That(rr.Value.GetObject<T>().Equals(enumer.Current));
            }
        }
    }
}
