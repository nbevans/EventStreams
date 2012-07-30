using System;

namespace EventStreams.Core {
    public static class InvocationExtensions {
        public static void Invoke<TEventArgs>(this StreamedEventHandler<TEventArgs> @event, TEventArgs e) where TEventArgs : StreamedEventArgs {
            Invoke(@event, e, new StreamingContext { Projecting = false });
        }

        internal static void Invoke<TEventArgs>(this StreamedEventHandler<TEventArgs> @event, TEventArgs e, StreamingContext context) where TEventArgs : StreamedEventArgs {
            if (@event != null)
                @event(e, context);
        }

        public static void Invoke<TEventArgs>(this TEventArgs e, StreamedEventHandler<TEventArgs> @event) where TEventArgs : StreamedEventArgs {
            Invoke(e, @event, new StreamingContext { Projecting = false });
        }

        internal static void Invoke<TEventArgs>(this TEventArgs e, StreamedEventHandler<TEventArgs> @event, StreamingContext context) where TEventArgs : StreamedEventArgs {
            if (e == null) throw new ArgumentNullException("e");
            Invoke(@event, e, context);
        }
    }
}
