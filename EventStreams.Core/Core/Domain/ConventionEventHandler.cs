﻿using System;

namespace EventStreams.Core.Domain {
    public class ConventionEventHandler<TAggregateRoot> : EventHandler<TAggregateRoot> where TAggregateRoot : class, IObserver<EventArgs>, new() {
        // Note: Because this is a generic type, each ConventionEventHandler per TAggregateRoot type will have its own static state.
        //       Therefore, the HandleMethodInvocationCache will be a different instance per TAggregateRoot type. Handy.
        //       Originally, the HandleMethodInvocationCache was based on an outer and inner cache.
        //       But leaning on the CLR behaviour of static and generics in this way will yield better performance.
        private static readonly HandleMethodInvocationCache<TAggregateRoot> _methodCache =
            new HandleMethodInvocationCache<TAggregateRoot>();

        public ConventionEventHandler(TAggregateRoot owner)
            : base(owner) { }

        public override void OnNext(EventArgs value) {
            base.OnNext(value);

            Action<object, EventArgs> method;
            if (_methodCache.TryGetMethod(value, out method))
                method.Invoke(Owner, value);
            else
                ThrowEventHandlerNotFound(value);
        }
    }
}