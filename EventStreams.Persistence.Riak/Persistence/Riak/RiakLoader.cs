using System;
using System.Linq;
using System.Collections.Generic;

namespace EventStreams.Persistence.Riak {
    using Core;
    using Serialization.Events;

    public class RiakLoader : ILoader {
        private readonly IEventReader _eventReader;

        public RiakLoader(IEventReader eventReader) {
            if (eventReader == null) throw new ArgumentNullException("eventReader");
            _eventReader = eventReader;
        }

        public IEnumerable<IStreamedEvent> Load(Guid identity) {
            return Enumerable.Empty<IStreamedEvent>();
        }
    }
}
