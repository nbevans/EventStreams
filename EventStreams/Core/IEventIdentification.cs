using System;

namespace EventStreams.Core
{
    public interface IEventIdentification
    {
        Guid Id { get; }

        string Name { get; }
    }
}
