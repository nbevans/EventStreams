using System;

namespace EventStreams.Core.Domain {
    public class ConventionEventHandler<TEventSourced> : EventHandler<TEventSourced> where TEventSourced : class, IEventSourced, new() {
        // Note: Because this is a generic type, each ConventionEventHandler per TEventSourced type will have its own static state.
        //       Therefore, the HandleMethodInvocationCache will be a different instance per TAggregateRoot type. Handy.
        //       Originally, the HandleMethodInvocationCache was based on an outer and inner cache.
        //       But leaning on the CLR behaviour of static and generics in this way will yield better performance.
        private static readonly HandleMethodInvocationCache<TEventSourced> _methodCache =
            new HandleMethodInvocationCache<TEventSourced>();

        public ConventionEventHandler(TEventSourced owner)
            : base(owner, EventHandlerBehavior.Lossless) { }

        public ConventionEventHandler(TEventSourced owner, EventHandlerBehavior behavior)
            : base(owner, behavior) { }

        public override void OnNext(EventArgs value) {
            base.OnNext(value);

            Action<TEventSourced, EventArgs> method;
            if (_methodCache.TryGetMethod(value, out method))
                method.Invoke(Owner, value);
            else
                HandleEventHandlerNotFound(value);
        }
    }
}
