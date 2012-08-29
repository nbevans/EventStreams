namespace EventStreams.Persistence.Serialization.Events {
    public struct EventReaderWriterPair : IEventReaderWriterPair {
        public static readonly IEventReaderWriterPair Json =
            new EventReaderWriterPair(
                new JsonEventReader(),
                new JsonEventWriter());

        public static readonly IEventReaderWriterPair Null =
            new EventReaderWriterPair(
                new NullEventReader(),
                new NullEventWriter());

        public IEventReader Reader { get; private set; }
        public IEventWriter Writer { get; private set; }

        public EventReaderWriterPair(IEventReader reader, IEventWriter writer)
            : this() {
            
            Reader = reader;
            Writer = writer;
        }
    }
}