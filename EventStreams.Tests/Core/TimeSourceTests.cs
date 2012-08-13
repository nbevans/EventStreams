using System;

using NUnit.Framework;

namespace EventStreams.Core {

    [TestFixture]
    public class TimeSourceTests {

        [Test]
        public void Expect_the_time_source_to_blindly_increment_by_100_ms_regardless_of_actual_time_passed_per_call() {
            var tmp = TimeSource.UtcNow;
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(100));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(200));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(300));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(400));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(500));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(600));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(700));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(800));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(900));
            Assert.That(TimeSource.UtcNow == tmp.AddMilliseconds(1000));
        }
    }
}
