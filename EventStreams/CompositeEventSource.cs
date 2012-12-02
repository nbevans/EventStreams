using System;
using System.Collections.Generic;

namespace EventStreams {
    /// <summary>
    /// A composition of multiple <see cref="EventSource"/> instances, each representing a logically separate source.
    /// This type acts as a facade interface by selecting the appropriate <see cref="EventSource"/> for a particular requested type of write model or read model.
    /// </summary>
    public class CompositeEventSource : IEventSource {
        private readonly Dictionary<Type, object> _sources =
            new Dictionary<Type, object>();

        public CompositeEventSource Add<TModel>(EventSource eventSource) where TModel : class, new() {
            _sources.Add(typeof(TModel), eventSource);
            return this;
        }

        /// <summary>
        /// Creates a new write model object with a unique identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be created.</typeparam>
        /// <returns>The newly created write model.</returns>
        public TWriteModel Create<TWriteModel>() where TWriteModel : class, IObservable<EventArgs>, new() {
            return GetSource<TWriteModel>().Create<TWriteModel>();
        }

        /// <summary>
        /// Creates a new write model object with a specific unique identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be created.</typeparam>
        /// <param name="identity">The identity of the event stream to be created.</param>
        /// <returns>The newly created write model.</returns>
        public TWriteModel Create<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new() {
            return GetSource<TWriteModel>().Create<TWriteModel>(identity);
        }

        /// <summary>
        /// Reads a specific event stream and projects it onto a read model implementation.
        /// </summary>
        /// <typeparam name="TReadModel">The of read model to be used for the projection.</typeparam>
        /// <param name="identity">The identity of the event stream to be read.</param>
        /// <returns>The projected read model.</returns>
        public TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, new() {
            return GetSource<TReadModel>().Read<TReadModel>(identity);
        }

        /// <summary>
        /// Opens an existing write model with a specific identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be opened.</typeparam>
        /// <param name="identity">The identity of the event stream to be opened or created.</param>
        /// <returns>The opened write model.</returns>
        public TWriteModel Open<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new() {
            return GetSource<TWriteModel>().Open<TWriteModel>(identity);
        }

        /// <summary>
        /// Opens or creates a write model with a specific identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be opened or created.</typeparam>
        /// <param name="identity">The identity of the event stream to be opened or created.</param>
        /// <returns>The write model that was either opened or created.</returns>
        public TWriteModel OpenOrCreate<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new() {
            return GetSource<TWriteModel>().OpenOrCreate<TWriteModel>(identity);
        }

        private EventSource GetSource<TModel>() where TModel : class, new() {
            object source;
            if (_sources.TryGetValue(typeof(TModel), out source))
                return (EventSource)source;

            throw new InvalidOperationException(
                string.Format("An event source for the '{0}' model type is not available.",
                              typeof(TModel).FullName));
        }
    }
}
