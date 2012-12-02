using System;

namespace EventStreams {
    /// <summary>
    /// A component that allows write models (WM) and read models (RM) to be sourced with observation and lifetime scope management from the underlying store.
    /// </summary>
    public interface IEventSource {
        /// <summary>
        /// Creates a new write model object with a unique identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be created.</typeparam>
        /// <returns>The newly created write model.</returns>
        TWriteModel Create<TWriteModel>() where TWriteModel : class, IObservable<EventArgs>, new();

        /// <summary>
        /// Creates a new write model object with a specific unique identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be created.</typeparam>
        /// <param name="identity">The identity of the event stream to be created.</param>
        /// <returns>The newly created write model.</returns>
        TWriteModel Create<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new();

        /// <summary>
        /// Reads a specific event stream and projects it onto a read model implementation.
        /// </summary>
        /// <typeparam name="TReadModel">The of read model to be used for the projection.</typeparam>
        /// <param name="identity">The identity of the event stream to be read.</param>
        /// <returns>The projected read model.</returns>
        TReadModel Read<TReadModel>(Guid identity) where TReadModel : class, new();

        /// <summary>
        /// Opens an existing write model with a specific identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be opened.</typeparam>
        /// <param name="identity">The identity of the event stream to be opened or created.</param>
        /// <returns>The opened write model.</returns>
        TWriteModel Open<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new();

        /// <summary>
        /// Opens or creates a write model with a specific identity.
        /// </summary>
        /// <typeparam name="TWriteModel">The type of write model to be opened or created.</typeparam>
        /// <param name="identity">The identity of the event stream to be opened or created.</param>
        /// <returns>The write model that was either opened or created.</returns>
        TWriteModel OpenOrCreate<TWriteModel>(Guid identity) where TWriteModel : class, IObservable<EventArgs>, new();
    }
}