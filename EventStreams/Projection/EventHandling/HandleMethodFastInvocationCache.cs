using System;
using System.Collections.Concurrent;

namespace EventStreams.Projection.EventHandling {
    internal static class HandleMethodFastInvocationCache {
        private static readonly ConcurrentDictionary<Type, HandleMethodFastInvocation> _cache =
            new ConcurrentDictionary<Type, HandleMethodFastInvocation>();

        public static HandleMethodFastInvocation Get(Type targetType) {
            return _cache.GetOrAdd(targetType, t => new HandleMethodFastInvocation(t));
        }
    }
}
