using System;
using System.Collections.Generic;
using System.Linq;

using CorrugatedIron;
using CorrugatedIron.Models;

namespace EventStreams.Persistence.Riak.Committers {

    internal class ConservativeCommit<TObject> : CommitBase<TObject> where TObject : class {

        public ConservativeCommit(IRiakClient riakClient, string bucket, IEnumerable<TObject> objects, Func<TObject, string> keySelector)
            : base(riakClient, bucket, objects, keySelector) { }

        public override void Commit() {
            Put();
            Wireup();
        }

        /// <summary>
        /// Puts the Riak objects onto the node(s).
        /// </summary>
        private void Put() {
            // ReSharper disable PossibleMultipleEnumeration

            var putResults = RiakClient.Put(RiakObjects, new RiakPutOptions { IfNoneMatch = true });
            var failures = putResults.Where(rr => !rr.IsSuccess);

            if (failures.Any()) {
                // Rollback (delete) any of the objects that may have been successfully created.
                //
                // It is safe to delete these because they were only JUST created on the above Put() line.
                // And since they were successfully added (IsSuccess == true) then it was definitely this
                // thread that created them, therefore nobody else knows about them as they haven't even
                // been referenced in the linked list yet.
                var successes = putResults.Where(rr => rr.IsSuccess);
                var rollbackResults = RiakClient.Delete(successes.Select(rr => rr.Value.ToRiakObjectId()));

                //
                throw new RiakObjectCreationPersistenceException(
                    string.Format(Resources.ExceptionStrings.Object_creation_pre_commitment_failed, failures.Count(), putResults.Count()),
                    putResults, rollbackResults);
            }

            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Wires up the Put()'d objects by updating the pointers and the last object's pointer to point to the tip of the new chain of objects.
        /// </summary>
        private void Wireup() {
            var ptrs = new Pointers(RiakClient, Bucket);

            try {
                // Creates a new head if this is a fresh bucket.
                // The new head is set to pointer to the first object in chain.
                var head = ptrs.FollowHead();
                if (head == null) {
                    ptrs.CreateHead(RiakObjects.First().ToRiakObjectId());
                }

                // Set last object (tail - 1) to point to the first object in chain.
                var last = ptrs.FollowTail();
                if (last != null) {
                    var rr = RiakClient.Get(last);
                    if (rr.IsSuccess) {
                        var ro = rr.Value;
                        ro.LinkTo(RiakObjects.First(), LinkNames.Pointer);
                        RiakClient.Put(ro, new RiakPutOptions { IfNotModified = true });

                    } else {
                        throw new RiakObjectWireupPersistenceException(
                            string.Format(Resources.ExceptionStrings.Object_wireup_failed, RiakObjects.Length));
                    }
                }

                // Set tail to point to the last object in chain.
                ptrs.UpdateTail(RiakObjects.Last().ToRiakObjectId());

            } catch (RiakPointerPersistenceException x) {
                throw new RiakObjectWireupPersistenceException(
                    string.Format(Resources.ExceptionStrings.Object_wireup_failed, RiakObjects.Length),
                    x);
            }
        }
    }
}
