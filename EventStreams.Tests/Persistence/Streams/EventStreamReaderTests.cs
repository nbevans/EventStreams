using System;
using System.Linq;
using System.IO;

using NUnit.Framework;

namespace EventStreams.Persistence.Streams {
    using Serialization.Events;
    using Resources;

    [TestFixture]
    internal class EventStreamReaderTests {

        [Test]
        public void Given_first_set_when_read_back_then_object_model_is_as_expected() {
            using (var ms = new MemoryStream()) {
                ResourceProvider.AppendTo(ms, "First.e");
                ms.Position = 0;

                using (var esr = new EventStreamReader(ms, new NullEventReader())) {
                    var firstExpected = MockEventStreams.First.ElementAt(0);
                    var firstActual = esr.Next();

                    Assert.AreEqual(firstActual.Id, firstExpected.Id);
                    Assert.AreEqual(firstActual.Timestamp, firstExpected.Timestamp);
                    Assert.AreEqual(firstActual.Arguments.GetType(), firstExpected.Arguments.GetType());

                    var secondExpected = MockEventStreams.First.ElementAt(1);
                    var secondActual = esr.Next();

                    Assert.AreEqual(secondActual.Id, secondExpected.Id);
                    Assert.AreEqual(secondActual.Timestamp, secondExpected.Timestamp);
                    Assert.AreEqual(secondActual.Arguments.GetType(), secondExpected.Arguments.GetType());
                }
            }
        }

        [Test]
        public void Given_first_and_second_set_when_fourth_item_is_artificially_corrupted_and_read_back_then_it_will_throw_on_fourth_iteration() {
            using (var ms = new MemoryStream()) {
                ResourceProvider.AppendTo(ms, "First_and_second.e");
                ms.Position = 0;

                var i = 0;
                Action<EventStreamReaderState> corruptor = s => {
                    if (s == EventStreamReaderState.HeadIndicator)
                        i++;

                    if (s == EventStreamReaderState.Hash && i == 4) {
                        // ReSharper disable AccessToDisposedClosure
                        ms.Write(new byte[] { 0, 0, 0 }, 0, 3);
                        ms.Position -= 3;
                        // ReSharper restore AccessToDisposedClosure
                    }
                };

                using (var esr = new EventStreamReader(ms, new NullEventReader(), corruptor, null)) {
                    // ReSharper disable AccessToDisposedClosure
                    Assert.DoesNotThrow(() => esr.Next());
                    Assert.DoesNotThrow(() => esr.Next());
                    Assert.DoesNotThrow(() => esr.Next());
                    Assert.Throws<HashVerificationPersistenceException>(() => esr.Next());
                    // ReSharper restore AccessToDisposedClosure
                }
            }
        }

        [Test]
        public void Given_first_and_second_set_when_artificially_truncated_and_read_back_then_it_will_throw_on_second_iteration() {
            using (var ms = new MemoryStream()) {
                ResourceProvider.AppendTo(ms, "First_and_second.e", 7 /* magic sauce to truncate by an arbitrary 7 bytes */);
                ms.Position = 0;

                using (var esr = new EventStreamReader(ms, new NullEventReader())) {
                    // ReSharper disable AccessToDisposedClosure
                    Assert.DoesNotThrow(() => esr.Next());
                    Assert.DoesNotThrow(() => esr.Next());
                    Assert.DoesNotThrow(() => esr.Next());
                    Assert.Throws<TruncationVerificationPersistenceException>(() => esr.Next());
                    // ReSharper restore AccessToDisposedClosure
                }
            }
        }
    }
}
