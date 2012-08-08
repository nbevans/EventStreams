using System;

namespace EventStreams.Core {
    public static class StreamedEventExtensions {
        public static TEventArgs ArgsOrNull<TEventArgs>(this IStreamedEvent streamedEvent) where TEventArgs : EventArgs {
            return streamedEvent.Arguments as TEventArgs;
        }

        public static TEventArgs Args<TEventArgs>(this IStreamedEvent streamedEvent) where TEventArgs : EventArgs {
            var tmp = streamedEvent.Arguments as TEventArgs;
            if (tmp != null)
                return tmp;

            throw new InvalidOperationException(
                "The arguments from the streamed event cannot be fetched because they are not of the requested type.");
        }
    }
}
