using System;
using System.Collections.Generic;

namespace EventStreams.Core.Domain {
    public class DelegatedEventHandler<TAggregateRoot> : AggregateRootEventHandlerBase<TAggregateRoot> where TAggregateRoot : class, new() {
        private readonly Dictionary<Type, Action<IStreamedEvent>> _handlers = new Dictionary<Type, Action<IStreamedEvent>>();

        public DelegatedEventHandler(TAggregateRoot owner)
            : base(owner) { }

        public DelegatedEventHandler<TAggregateRoot> Bind<T>(Action<IStreamedEvent> handler) {
            _handlers.Add(typeof(T), handler);
            return this;
        }

        public override void OnNext(IStreamedEvent value) {
            base.OnNext(value);

            Action<IStreamedEvent> handler;
            if (_handlers.TryGetValue(value.Arguments.GetType(), out handler))
                handler(value);
            else
                ThrowEventHandlerNotFound(value);
        }
    }
}
