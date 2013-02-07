using System;
using System.Linq;

using CorrugatedIron;
using CorrugatedIron.Models;

namespace EventStreams.Persistence.Riak.Committers {

    internal class Pointers {
        private readonly IRiakClient _riakClient;
        private readonly string _bucket;

        public Pointers(IRiakClient riakClient, string bucket) {
            if (riakClient == null) throw new ArgumentNullException("riakClient");
            if (bucket == null) throw new ArgumentNullException("bucket");
            _riakClient = riakClient;
            _bucket = bucket;
        }

        public RiakObjectId FollowHead() {
            return FollowPointer(PointerKeys.Head, LinkNames.Pointer);
        }

        public RiakObjectId FollowTail() {
            return FollowPointer(PointerKeys.Tail, LinkNames.Pointer);
        }

        public void CreateHead(RiakObjectId next) {
            var head = new RiakObject(_bucket, PointerKeys.Head);
            head.LinkTo(next, LinkNames.Pointer);

            var rr = _riakClient.Put(head, new RiakPutOptions { IfNoneMatch = true });
            if (!rr.IsSuccess)
                throw new RiakHeadPointerPersistenceException(
                    Resources.ExceptionStrings.Head_pointer_update_failed,
                    rr);
        }

        public void UpdateTail(RiakObjectId prev) {
            var success = false;
            var rr = _riakClient.Get(_bucket, PointerKeys.Tail);

            RiakObject ro = null;
            if (rr.IsSuccess)
                ro = rr.Value;
            else if (rr.ResultCode == ResultCode.NotFound)
                ro = new RiakObject(_bucket, PointerKeys.Tail);

            if (ro != null) {
                ro.LinkTo(prev, LinkNames.Pointer);

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

        private RiakObjectId FollowPointer(string key, string linkName) {
            var rr = _riakClient.Get(_bucket, key);

            if (rr.IsSuccess) {
                var link = rr.Value.Links.SingleOrDefault(l => l.Tag.Equals(linkName, StringComparison.Ordinal));
                if (link != null)
                    return new RiakObjectId(link.Bucket, link.Key);

                // Highly unusual.
                // Was the pointer object created or tampered with by some foreign system?
                throw new RiakPersistenceException(
                    string.Format(
                        Resources.ExceptionStrings.Corrupt_pointer,
                        key, linkName));
            }

            return null;
        }
    }
}
