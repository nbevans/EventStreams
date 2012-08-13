using System;
using NUnit.Framework;

namespace EventStreams {
    using Core;

    [SetUpFixture]
    public class TimeSourceSetUp {

        private DateTime _now =
            new DateTime(2012, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        [SetUp]
        public void SetUp() {
            TimeSource.Set(() => (_now = _now.AddMilliseconds(100)));
        }

        [TearDown]
        public void TearDown() {
            TimeSource.Clear();
        }
    }
}
