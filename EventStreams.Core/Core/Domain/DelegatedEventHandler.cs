using System;
using System.Collections.Generic;

namespace EventStreams.Core.Domain {
    [Obsolete]
    internal class DelegatedEventHandler<TWriteModel> : EventHandler<TWriteModel> where TWriteModel : class, new() {
        private readonly Dictionary<Type, Action<EventArgs>> _handlers = new Dictionary<Type, Action<EventArgs>>();

        public DelegatedEventHandler(TWriteModel owner)
            : base(owner, EventHandlerBehavior.Lossless) { }

        public DelegatedEventHandler(TWriteModel owner, EventHandlerBehavior behavior)
            : base(owner, behavior) { }

        public DelegatedEventHandler<TWriteModel> Bind<T>(Action<EventArgs> handler) {
            _handlers.Add(typeof(T), handler);
            return this;
        }

        public override void OnNext(EventArgs value) {
            base.OnNext(value);

            Action<EventArgs> handler;
            if (_handlers.TryGetValue(value.GetType(), out handler))
                handler(value);
            else
                HandleEventHandlerNotFound(value);
        }
    }
}
