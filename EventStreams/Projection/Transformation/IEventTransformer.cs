//using System;
//using System.Collections.Generic;

//namespace EventStreams.Projection.Transformation
//{
//    using Core;

//    public interface IEventTransformer
//    {
//        IEnumerable<IEvent<TAggregateRoot>> Transform<TAggregateRoot>(IEvent<TAggregateRoot> candidateEvent)
//            where TAggregateRoot : class, new();
//    }
//}