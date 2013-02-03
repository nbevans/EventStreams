using System;
using System.Linq;
using System.Collections.Generic;

using CorrugatedIron;

namespace EventStreams.Persistence.Riak {
    using Core;
    using Serialization.Events;

    public class RiakLoader : ILoader {
        private readonly IRiakClient _riakClient;
        private readonly IEventReader _eventReader;

        public RiakLoader(IRiakClient riakClient, IEventReader eventReader) {
            if (riakClient == null) throw new ArgumentNullException("riakClient");
            if (eventReader == null) throw new ArgumentNullException("eventReader");
            _riakClient = riakClient;
            _eventReader = eventReader;
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            return Enumerable.Empty<IStreamedEvent>();
        }
    }
}
