using System;

namespace EventStreams.Persistence.Riak {
    internal static class ArrayExtensions {
        public static T Previous<T>(this T[] array, int currentIndex) {
            return currentIndex > 0 ? array[currentIndex - 1] : default(T);
        }

        public static T Next<T>(this T[] array, int currentIndex) {
            return currentIndex < array.Length - 1 ? array[currentIndex + 1] : default(T);
        }
    }
}
