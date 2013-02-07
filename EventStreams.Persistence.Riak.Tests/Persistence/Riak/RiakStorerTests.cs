using System;

using NUnit.Framework;

using CorrugatedIron.Util;

namespace EventStreams.Persistence.Riak {
    using Core;
    using Serialization.Events;

    [TestFixture]
    public class RiakStorerTests {

        private readonly Guid _bucketId = new Guid("d83fee6b-bf9d-4b78-806f-6fa75ab28777");

        private readonly StreamedEvent _eventA = new StreamedEvent(
            new Guid("b84e1d6e-8b60-44e9-9216-b8bac82abc59"),
            new DateTime(2013, 1, 29),
            new EventArgs());

        private readonly StreamedEvent _eventB = new StreamedEvent(
            new Guid("30f421fb-1b53-4d04-9466-bbb90bc10ad6"),
            new DateTime(2013, 1, 29),
            new EventArgs());

        private readonly StreamedEvent _eventC = new StreamedEvent(
            new Guid("bed55f93-263c-457a-85cc-5212cd795904"),
            new DateTime(2013, 1, 29),
            new EventArgs());

        private readonly StreamedEvent _eventD = new StreamedEvent(
            new Guid("45738981-3523-4fe7-954f-f52f02739f3f"),
            new DateTime(2013, 1, 29),
            new EventArgs());

        [Test]
        public void foo() {
            var riakClient = TestingRiakClient.Get();
            var storer = new RiakStorer(riakClient, new NullEventWriter());

            riakClient.DeleteBucket(_bucketId.ToRiakIdentity(), RiakConstants.QuorumOptions.All);

            storer.Store(_bucketId, new[] { _eventA, _eventB, _eventC, _eventD });
        }
    }
}
