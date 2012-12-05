using System;

using Moq;

namespace EventStreams.Persistence.Streams {
    using Core;
    using Domain.Accounting.Events;

    internal class MockEventStreams {

        public static readonly IStreamedEvent[] First = new[] {
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("20F65C10-D7DE-43B0-A527-4CCC43496BFE") && f.Timestamp == DateTime.MinValue && f.Arguments == new PayeSalaryDeposited(100, "Acme Corp")),
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("685F85DA-07AC-4EB3-B7F5-F52BCD543E84") && f.Timestamp == DateTime.MinValue && f.Arguments == new MadePurchase(45, "Wine"))
        };

        public static readonly IStreamedEvent[] Second = new[] {
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("1012E630-8325-47D4-9393-DCD7F5940E09") && f.Timestamp == DateTime.MinValue && f.Arguments == new PayeSalaryDeposited(150, "Acme Corp")),
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("E19772BA-6DAE-408E-9F09-8561889C8154") && f.Timestamp == DateTime.MinValue && f.Arguments == new MadePurchase(25, "Cheese"))
        };

    }
}
