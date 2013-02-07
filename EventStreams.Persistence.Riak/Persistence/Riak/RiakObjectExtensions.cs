using System;
using System.Linq;

using CorrugatedIron.Models;

namespace EventStreams.Persistence.Riak {
    internal static class RiakObjectExtensions {
        /// <summary>
        /// Creates (or updates) a link to point to the designated object.
        /// </summary>
        /// <param name="ro">The Riak object whose link will be created or updated.</param>
        /// <param name="riakObjectId">The Riak object identifier to be linked to.</param>
        /// <param name="tag">The tag (or name) of the link.</param>
        public static void SingleLinkTo(this RiakObject ro, RiakObjectId riakObjectId, string tag) {
            SingleLinkTo(ro, riakObjectId.Bucket, riakObjectId.Key, tag);

        }

        /// <summary>
        /// Creates (or updates) a link to point to the designated object.
        /// </summary>
        /// <param name="ro">The Riak object whose link will be created or updated.</param>
        /// <param name="riakObject">The Riak object to be linked to.</param>
        /// <param name="tag">The tag (or name) of the link.</param>
        public static void SingleLinkTo(this RiakObject ro, RiakObject riakObject, string tag) {
            SingleLinkTo(ro, riakObject.Bucket, riakObject.Key, tag);
        }

        /// <summary>
        /// Creates (or updates) a link to point to the designated object.
        /// </summary>
        /// <param name="ro">The Riak object whose link will be created or updated.</param>
        /// <param name="bucket">The bucket of the object to be linked to.</param>
        /// <param name="key">The key of the object to be linked to.</param>
        /// <param name="tag">The tag (or name) of the link.</param>
        public static void SingleLinkTo(this RiakObject ro, string bucket, string key, string tag) {
            var link = ro.Links.SingleOrDefault(l => l.Tag.Equals(tag));
            if (link != null)
                ro.RemoveLink(link);

            ro.LinkTo(bucket, key, tag);
        }
    }
}
