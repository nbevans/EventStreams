using System;

namespace EventStreams.Core.Domain {
    public class ConventionEventHandler<TAggregateRoot> : AggregateRootEventHandlerBase<TAggregateRoot> where TAggregateRoot : class, IObserver<EventArgs>, new() {
        private readonly MethodInvocationCache _methodCache = new MethodInvocationCache();

        public ConventionEventHandler(TAggregateRoot owner)
            : base(owner) {

            _methodCache.Cache<TAggregateRoot>();
        }

        public override void OnNext(EventArgs value) {
            base.OnNext(value);

            Action<object, EventArgs> method;
            if (_methodCache.TryGetMethod<TAggregateRoot>(value, out method))
                method.Invoke(Owner, value);
            else
                ThrowEventHandlerNotFound(value);
        }
    }
}
