using System;
using System.Collections.Generic;

using CorrugatedIron;

namespace EventStreams.Persistence.Riak.Committers {
    using System.Linq;
    using CorrugatedIron.Models;

    internal abstract class CommitBase<TObject> where TObject : class {
        protected IRiakClient RiakClient { get; private set; }
        protected string Bucket { get; private set; }
        protected Func<TObject, string> KeySelector { get; private set; }
        protected TObject[] Objects { get; private set; }
        protected RiakObject[] RiakObjects { get; private set; }

        protected CommitBase(IRiakClient riakClient, string bucket, IEnumerable<TObject> objects, Func<TObject, string> keySelector) {
            if (riakClient == null) throw new ArgumentNullException("riakClient");
            if (bucket == null) throw new ArgumentNullException("bucket");
            if (objects == null) throw new ArgumentNullException("objects");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            RiakClient = riakClient;
            Bucket = bucket;
            Objects = objects.ToArray();
            KeySelector = keySelector;

            RiakObjects = PrepareObjects();
        }

        public abstract void Commit();

        /// <summary>
        /// Prepares the Riak objects and chains them together as a linked list.
        /// </summary>
        /// <returns>An array of <see cref="RiakObject"/> instances that are prepared for storage.</returns>
        private RiakObject[] PrepareObjects() {
            var objects = Objects.ToArray();
            var riakObjects = new RiakObject[objects.Length];

            for (var i = 0; i < objects.Length; ++i) {
                var obj = objects[i];

                var ro = new RiakObject(Bucket, KeySelector(obj), obj);
                var next = objects.Next(i);

                // Wire up object pointers to form the linked list.
                if (next != null)
                    ro.SingleLinkTo(Bucket, KeySelector(next), LinkNames.Pointer);

                //
                riakObjects[i] = ro;
            }

            // Special case for last in the chain, which must point to the "tail".
            riakObjects[objects.Length - 1].SingleLinkTo(Bucket, PointerKeys.Tail, LinkNames.Pointer);

            return riakObjects;
        }
    }
}