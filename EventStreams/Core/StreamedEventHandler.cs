namespace EventStreams.Core {

    public delegate void StreamedEventHandler<in TEventArgs>(TEventArgs e, StreamingContext context)
        where TEventArgs : StreamedEventArgs;

}