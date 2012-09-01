using System;
using System.Collections.Generic;

namespace EventStreams.Persistence {
    using Core;

    public interface ILoader {
        IEnumerable<IStreamedEvent> Load(Guid identity);
    }
}
