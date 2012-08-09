using System;
namespace EventStreams.Projection.Transformation {
    public interface IEventSequenceTransformer {
        IEventSequenceTransformer Bind<TEventTransformer>() where TEventTransformer : class, IEventTransformer, new();
    }
}
