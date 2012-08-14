using System;

namespace EventStreams.Core {
    public static class EventArgsExtensions {
        public static StreamedEvent ToStreamedEvent(this EventArgs args) {
            return new StreamedEvent(args);
        }

        public static StreamedEvent ToStreamedEvent(this EventArgs args, string identity) {
            return new StreamedEvent(new Guid(identity), args);
        }

        public static StreamedEvent ToStreamedEvent(this EventArgs args, Guid identity) {
            return new StreamedEvent(identity, args);
        }

        public static TEventArgs Assume<TEventArgs>(this EventArgs args) where TEventArgs : EventArgs {
            var tmp = args as TEventArgs;
            if (tmp != null)
                return tmp;

            throw new InvalidOperationException(
                "The arguments cannot be converted because they are not of the expected type.");
        }
    }
}
