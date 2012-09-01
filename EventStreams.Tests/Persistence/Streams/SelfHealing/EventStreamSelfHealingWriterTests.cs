using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace EventStreams.Persistence.Streams.SelfHealing {
    using Serialization.Events;
    using Resources;

    [TestFixture]
    internal class EventStreamSelfHealingWriterTests {

        [Test]
        public void Given_first_set_when_artificially_truncated_and_appended_to_with_second_set_then_it_will_not_throw_on_write() {
            using (var ms = new MemoryStream()) {
                ResourceProvider.AppendTo(ms, "First.e", 7 /* magic sauce to truncate by an arbitrary 7 bytes */);

                using (var esw = new EventStreamWriter(ms, new NullEventWriter()))
                using (var esshw = new EventStreamSelfHealingWriter(esw)) {
                    // ReSharper disable AccessToDisposedClosure
                    Assert.DoesNotThrow(() => esshw.Write(MockEventStreams.Second));
                    // ReSharper restore AccessToDisposedClosure

                    using (var msExpected = new MemoryStream())
                    using (var esrExpected = new EventStreamWriter(msExpected, new NullEventWriter())) {
                        esrExpected.Write(MockEventStreams.First.Take(1));
                        esrExpected.Write(MockEventStreams.Second);
                        Assert.AreEqual(ms.ReadStartToEnd(), msExpected.ReadStartToEnd());
                    }
                }
            }
        }
    }
}
