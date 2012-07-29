using System;
using System.Collections.Generic;

namespace EventStreams.Core {
    interface IPerformanceTestSuite {
        IEnumerable<Action> GetTests();
    }
}
