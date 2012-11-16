using System;

namespace EventStreams.Core {
    public static class StreamedEventExtensions {
        /// <summary>
        /// Gets the event arguments from a <see cref="IStreamedEvent"/> object and casts it to the assumed type. If the extraction fails then <code>null</code> will be returned.
        /// </summary>
        /// <typeparam name="TEventArgs">The assumed type of the event arguments.</typeparam>
        /// <param name="streamedEvent">The <see cref="IStreamedEvent"/> whose event arguments will be extracted.</param>
        /// <returns>The extracted <see cref="EventArgs"/> object in the requested type, or null if the extraction fails.</returns>
        public static TEventArgs ArgsOrNull<TEventArgs>(this IStreamedEvent streamedEvent) where TEventArgs : EventArgs {
            return streamedEvent.Arguments as TEventArgs;
        }

        /// <summary>
        /// Gets the event arguments from a <see cref="IStreamedEvent"/> object and casts it to the assumed type. If extraction fails then an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEventArgs">The assumed type of the event arguments.</typeparam>
        /// <param name="streamedEvent">The <see cref="IStreamedEvent"/> whose event arguments will be extracted.</param>
        /// <returns>The extracted <see cref="EventArgs"/> object in the requested type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the extraction has failed either because the event arguments object was null or the cast operation failed.</exception>
        public static TEventArgs Args<TEventArgs>(this IStreamedEvent streamedEvent) where TEventArgs : EventArgs {
            var tmp = streamedEvent.Arguments as TEventArgs;
            if (tmp != null)
                return tmp;

            throw new InvalidOperationException(
                "The arguments from the streamed event cannot be fetched because they are not of the requested type.");
        }
    }
}
