using System;

namespace EventStreams.Core.Domain {
    public class ConventionEventHandler<TAggregateRoot> : AggregateRootEventHandlerBase<TAggregateRoot> where TAggregateRoot : class, new() {
        private readonly MethodInvocationCache _methodCache = new MethodInvocationCache();

        public ConventionEventHandler(TAggregateRoot owner)
            : base(owner) {

            _methodCache.Cache<TAggregateRoot>();
        }

        public override void OnNext(IStreamedEvent value) {
            base.OnNext(value);

            Action<object, EventArgs> method;
            if (_methodCache.TryGetMethod<TAggregateRoot>(value, out method))
                method.Invoke(Owner, value.Arguments);
            else
                ThrowEventHandlerNotFound(value);
        }
    }
}
