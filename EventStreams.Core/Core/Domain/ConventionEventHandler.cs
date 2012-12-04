using System;

namespace EventStreams.Core.Domain {
    public class ConventionEventHandler<TModel> : EventHandler<TModel> where TModel : class, new() {
        // Note: Because this is a generic type, each ConventionEventHandler per TModel type will have its own static state.
        //       Therefore, the HandleMethodInvocationCache will be a different instance per TModel type. Handy.
        //       Originally, the HandleMethodInvocationCache was based on an outer and inner cache.
        //       But leaning on the CLR behaviour of static and generics in this way will yield better performance.
        private static readonly HandleMethodInvocationCache<TModel> _methodCache =
            new HandleMethodInvocationCache<TModel>();

        public ConventionEventHandler(TModel owner)
            : base(owner, EventHandlerBehavior.Lossless) { }

        public ConventionEventHandler(TModel owner, EventHandlerBehavior behavior)
            : base(owner, behavior) { }

        public override void OnNext(EventArgs value) {
            base.OnNext(value);

            Action<TModel, EventArgs> method;
            if (_methodCache.TryGetMethod(value, out method))
                method.Invoke(Owner, value);
            else
                HandleEventHandlerNotFound(value);
        }
    }
}
