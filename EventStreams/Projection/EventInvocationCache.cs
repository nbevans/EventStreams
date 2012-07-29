using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using EventStreams.Core;

namespace EventStreams.Projection {
    using StreamedEventAction = Action<object, IStreamedEvent, bool>;

    internal class EventInvocationCache {
        private readonly Dictionary<Type, Dictionary<Type, StreamedEventAction>> _cache =
            new Dictionary<Type, Dictionary<Type, StreamedEventAction>>();

        public void Cache<TAggregateRoot>() where TAggregateRoot : class, new() {
            if (_cache.ContainsKey(typeof(TAggregateRoot)))
                return;

            var events = typeof(TAggregateRoot).GetEvents(BindingFlags.Instance | BindingFlags.Public);
            foreach (var e in events) {
                if (e.EventHandlerType.IsGenericType && e.EventHandlerType.GetGenericTypeDefinition().Equals(typeof(StreamedEventHandler<>))) {
                    var genericArgs = e.EventHandlerType.GetGenericArguments().First();
                    EnsureCachedAndGet<TAggregateRoot>(genericArgs);
                }
            }
        }

        public StreamedEventAction Get<TAggregateRoot>(IStreamedEvent currentEvent) where TAggregateRoot : class, new() {
            return EnsureCachedAndGet<TAggregateRoot>(currentEvent.GetType());
        }

        private StreamedEventAction EnsureCachedAndGet<TAggregateRoot>(Type streamedEvent) where TAggregateRoot : class, new() {
            if (streamedEvent == null) throw new ArgumentNullException("streamedEvent");

            Dictionary<Type, StreamedEventAction> innerCache;
            if (!_cache.TryGetValue(typeof(TAggregateRoot), out innerCache)) {
                _cache.Add(typeof(TAggregateRoot), (innerCache = new Dictionary<Type, StreamedEventAction>()));
            }

            StreamedEventAction d;
            if (!innerCache.TryGetValue(streamedEvent, out d)) {
                var name = streamedEvent.Name;
                var fi = typeof(TAggregateRoot).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fi != null) {
                    var tmp = new TAggregateRoot();
                    var mcastDelegate = fi.GetValue(tmp) as MulticastDelegate;
                    if (mcastDelegate != null) {
                        d = DelegateHelper
                            .CreateOpenInstanceDelegate<StreamedEventAction>(
                                mcastDelegate.Method);

                        innerCache.Add(streamedEvent, d);

                    } else {
                        throw new InvalidOperationException(
                            string.Format(
                                "The '{0}' event exists on the '{1}' entity type but its delegate is of the wrong type. " +
                                "It must be a '{2}'. " +
                                "Alternatively, the event may be valid but the entity type has not subscribed a handler.",
                                name, typeof(TAggregateRoot), typeof(StreamedEventHandler<>)));
                    }

                } else {
                    throw new InvalidOperationException(
                        string.Format(
                            "The '{0}' event does not exist on the '{1}' entity type.",
                            name, typeof(TAggregateRoot)));
                }
            }

            return d;
        }
    }
}
