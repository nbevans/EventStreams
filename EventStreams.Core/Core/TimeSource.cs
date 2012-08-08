using System;

namespace EventStreams.Core {
    /// <summary>
    /// Provides the ability to override the current moment in time to facilitate testing.
    /// </summary>
    /// <remarks>
    /// Original idea by Ayende Rahien:
    /// http://ayende.com/Blog/archive/2008/07/07/Dealing-with-time-in-tests.aspx
    /// </remarks>
    public static class TimeSource {
        /// <summary>
        /// Gets the callback to be used to resolve the current moment in time.
        /// </summary>
        public static Func<DateTime> Source { get; private set; }

        /// <summary>
        /// Gets the current moment in time, in UTC.
        /// </summary>
        public static DateTime UtcNow {
            get { return Source(); }
        }

        static TimeSource() {
            Clear();
        }

        /// <summary>
        /// Sets the source to be used to resolve the current moment in time.
        /// </summary>
        /// <param name="source"></param>
        public static void Set(Func<DateTime> source) {
            if (source == null) throw new ArgumentNullException("source");
            Source = source;
        }

        /// <summary>
        /// Clears the source and reverts to the default.
        /// </summary>
        public static void Clear() {
            Source = () => DateTime.UtcNow;
        }
    }
}
