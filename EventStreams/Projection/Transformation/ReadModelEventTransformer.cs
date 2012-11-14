using System;
using System.Collections.Generic;

namespace EventStreams.Projection.Transformation {
    using Core;
    using ReadModelling;

    internal class ReadModelEventTransformer : IEventTransformer {
        private readonly IReadModel _readModel;

        public ReadModelEventTransformer(IReadModel readModel) {
            if (readModel == null) throw new ArgumentNullException("readModel");
            _readModel = readModel;
        }

        public DateTime Chronology { get { return DateTime.MaxValue; } }
        public IEnumerable<IStreamedEvent> Transform<TAggregateRoot>(IStreamedEvent candidateEvent) {
            return _readModel.Read<TAggregateRoot>(candidateEvent);
        }
    }
}
