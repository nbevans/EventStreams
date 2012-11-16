namespace EventStreams.Core.Domain {
    /// <summary>
    /// Indicates the behavior by which event handling is expected to occur.
    /// </summary>
    public enum EventHandlerBehavior {
        /// <summary>
        /// Events will be handled losslessly i.e. there must be a corresponding handling method on the type.
        /// This is the default.
        /// </summary>
        Lossless,

        /// <summary>
        /// Events will be handled lossily i.e. there is no requirement that the type has a corresponding handling method.
        /// Typically this is used when projecting a read model.
        /// </summary>
        Lossy
    }
}