using System;
using System.Collections.Generic;
using System.Linq;

using CorrugatedIron;
using CorrugatedIron.Models;

namespace EventStreams.Persistence.Riak {
    using Core;

    internal class CommitContext {
        private readonly IRiakClient _riakClient;
        private readonly string _bucket;
        private readonly RiakObject[] _objects;

        public CommitContext(IRiakClient riakClient, string bucket, IEnumerable<IStreamedEvent> events) {
            if (riakClient == null) throw new ArgumentNullException("riakClient");
            if (bucket == null) throw new ArgumentNullException("bucket");
            
            _riakClient = riakClient;
            _bucket = bucket;

            _objects = PrepareObjects(events.ToArray());
        }

        public void Commit() {
            Put();
            Wireup();
        }

        private RiakObject[] PrepareObjects(IStreamedEvent[] events) {
            var objects = new RiakObject[events.Length];

            for (var i = 0; i < events.Length; ++i) {
                var e = events[i];

                var ro = new RiakObject(_bucket, e.ToRiakIdentity(), e);
                var prev = events.Previous(i);
                var next = events.Next(i);

                //
                if (prev != null)
                    ro.LinkTo(_bucket, prev.ToRiakIdentity(), "prev");
                
                if (next != null)
                    ro.LinkTo(_bucket, next.ToRiakIdentity(), "next");

                //
                objects[i] = ro;
            }

            return objects;
        }

        private void Put() {
            // ReSharper disable PossibleMultipleEnumeration

            var results = _riakClient.Put(_objects, new RiakPutOptions { IfNoneMatch = true });

            if (results.Any(rr => !rr.IsSuccess))
                throw new RiakObjectCreationPersistenceException(
                    string.Format(Resources.ExceptionStrings.Object_creation_pre_commitment_failed, results.Count()),
                    results);

            // ReSharper restore PossibleMultipleEnumeration
        }

        private void Wireup() {
            var nav = new BucketPointers(_riakClient, _bucket);

            try {
                var head = nav.GetHead();

                if (head == null)
                    nav.CreateHead(_objects.First().ToRiakObjectId());

                nav.UpdateTail(_objects.Last().ToRiakObjectId());

            } catch (RiakPointerPersistenceException x) {
                throw new RiakObjectWireupPersistenceException(
                    string.Format(Resources.ExceptionStrings.Object_wireup_failed, _objects.Length),
                    x);
            }
        }
    }
}
