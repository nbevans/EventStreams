using System;
using System.Linq;

using CorrugatedIron;
using CorrugatedIron.Models;

namespace EventStreams.Persistence.Riak {

    internal class BucketPointers {
        private const string HeadKey = "head";
        private const string TailKey = "tail";
        private const string PrevLink = "prev";
        private const string NextLink = "next";

        private readonly IRiakClient _riakClient;
        private readonly string _bucket;

        public BucketPointers(IRiakClient riakClient, string bucket) {
            if (riakClient == null) throw new ArgumentNullException("riakClient");
            if (bucket == null) throw new ArgumentNullException("bucket");
            _riakClient = riakClient;
            _bucket = bucket;
        }

        public RiakObjectId GetHead() {
            return GetPointer(HeadKey, NextLink);
        }

        public RiakObjectId GetTail() {
            return GetPointer(TailKey, PrevLink);
        }

        public void CreateHead(RiakObjectId next) {
            var head = new RiakObject(_bucket, HeadKey);
            head.LinkTo(next, NextLink);

            var rr = _riakClient.Put(head, new RiakPutOptions { IfNoneMatch = true });
            if (!rr.IsSuccess)
                throw new RiakHeadPointerPersistenceException(
                    Resources.ExceptionStrings.Head_pointer_update_failed,
                    rr);
        }

        public void UpdateTail(RiakObjectId prev) {
            var success = false;
            var rr = _riakClient.Get(_bucket, TailKey);

            RiakObject ro = null;
            if (rr.IsSuccess)
                ro = rr.Value;
            else if (rr.ResultCode == ResultCode.NotFound)
                ro = new RiakObject(_bucket, TailKey);

            if (ro != null) {
                ro.LinkTo(prev, PrevLink);

                rr = _riakClient.Put(
                    ro,
                    new RiakPutOptions {
                        IfNotModified = rr.IsSuccess,
                        IfNoneMatch = !rr.IsSuccess && rr.ResultCode == ResultCode.NotFound
                    });

                success = rr.IsSuccess;
            }

            if (!success)
                throw new RiakTailPointerPersistenceException(
                    Resources.ExceptionStrings.Tail_pointer_update_failed,
                    rr);
        }

        private RiakObjectId GetPointer(string key, string linkName) {
            var rr = _riakClient.Get(_bucket, key);

            if (rr.IsSuccess) {
                var link = rr.Value.Links.SingleOrDefault(l => l.Tag.Equals(linkName, StringComparison.Ordinal));
                if (link != null)
                    return new RiakObjectId(link.Bucket, link.Key);

                // Highly unusual.
                // Was the pointer object created by some other system?
                throw new RiakPersistenceException(
                    string.Format(
                        "Pointer object ({0}) exists but it does not contain the expected link ({1}).",
                        key, linkName));
            }

            return null;
        }
    }
}
