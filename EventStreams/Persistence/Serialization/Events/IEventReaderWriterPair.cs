namespace EventStreams.Persistence.Serialization.Events {
    public interface IEventReaderWriterPair {
        IEventReader Reader { get; }
        IEventWriter Writer { get; }
    }
}