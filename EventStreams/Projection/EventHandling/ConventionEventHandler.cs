using System;

namespace EventStreams.Projection.EventHandling {
    public class ConventionEventHandler : EventHandler {
        private readonly HandleMethodFastInvocation _methodCache;

        public ConventionEventHandler(object owner, EventHandlerBehavior behavior = EventHandlerBehavior.Lossless)
            : base(owner, behavior) {

            _methodCache = HandleMethodFastInvocationCache.Get(owner.GetType());
        }

        public override void OnNext(EventArgs value) {
            base.OnNext(value);

            Action<object, EventArgs> method;
            if (_methodCache.TryGetMethod(value, out method))
                method.Invoke(Owner, value);
            else
                HandleEventHandlerNotFound(value);
        }
    }
}
