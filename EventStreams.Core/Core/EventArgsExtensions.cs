using System;

namespace EventStreams.Core {
    public static class EventArgsExtensions {
        /// <summary>
        /// Transforms an <see cref="EventArgs"/> into a <see cref="StreamedEvent"/> with a new identifier and fresh timestamp.
        /// </summary>
        /// <param name="args">The event arguments to transformed.</param>
        /// <returns>A <see cref="StreamedEvent"/> representing the transformed event arguments.</returns>
        public static StreamedEvent ToStreamedEvent(this EventArgs args) {
            return new StreamedEvent(args);
        }

        /// <summary>
        /// Assumes that a <see cref="EventArgs"/> object is of a particular type.
        /// </summary>
        /// <typeparam name="TEventArgs">The type to be assumed.</typeparam>
        /// <param name="args">The event arguments to be casted.</param>
        /// <returns>The casted <typeparamref name="TEventArgs"/> object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the event arguments cannot be casted.</exception>
        public static TEventArgs Assume<TEventArgs>(this EventArgs args) where TEventArgs : EventArgs {
            var tmp = args as TEventArgs;
            if (tmp != null)
                return tmp;

            throw new InvalidOperationException(
                "The arguments cannot be converted because they are not of the expected type.");
        }
    }
}
