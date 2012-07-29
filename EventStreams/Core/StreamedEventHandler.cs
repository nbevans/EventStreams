namespace EventStreams.Core {

    public delegate void StreamedEventHandler<TEventArgs>(TEventArgs e, bool projecting)
        where TEventArgs : StreamedEventArgs;

}