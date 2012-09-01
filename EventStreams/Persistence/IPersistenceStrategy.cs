using System;

namespace EventStreams.Persistence {
    public interface IPersistenceStrategy : IStorer, ILoader { }
}
