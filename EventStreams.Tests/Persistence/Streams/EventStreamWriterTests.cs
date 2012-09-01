using System;
using System.IO;

using NUnit.Framework;

namespace EventStreams.Persistence.Streams {
    using Serialization.Events;
    using Resources;

    [TestFixture]
    internal class EventStreamWriterTests {

        [Test]
        public void Given_first_set_when_read_back_then_output_is_as_expected() {
            using (var ms = new MemoryStream()) {
                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    esw.Write(MockEventStreams.First);
                    Assert.AreEqual(ms.ReadStartToEnd(), ResourceProvider.Get("First.e"));
                }
            }
        }

        [Test]
        public void Given_first_set_when_appended_to_with_second_set_then_hashes_continue_from_previous_hash() {
            using (var ms = new MemoryStream()) {
                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    esw.Write(MockEventStreams.First);
                    esw.Write(MockEventStreams.Second);
                    Assert.AreEqual(ms.ReadStartToEnd(), ResourceProvider.Get("First_and_second.e"));
                }
            }
        }

        [Test]
        public void Given_first_set_when_appended_to_with_first_set_again_then_hashing_continues_from_item_two_in_the_file() {
            using (var ms = new MemoryStream()) {
                ResourceProvider.AppendTo(ms, "First.e");

                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    esw.Write(MockEventStreams.First);
                    Assert.AreEqual(ms.ReadStartToEnd(), ResourceProvider.Get("First_with_hash_seed.e"));
                }
            }
        }

        [Test]
        public void Given_a_malformed_supposedly_hash_seeded_stream_when_appended_to_with_first_set_then_it_will_throw() {
            using (var ms = new MemoryStream()) {
                // This is just some malformed junk that is of sufficient length and structure to produce the behaviour.
                ms.Write(new byte[20], 0, 20); // hash
                ms.Write(new byte[4], 0, 4); // tail record length
                ms.Write(new byte[] { 0x02 }, 0, 1); // (an incorrect) tail indicator byte

                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    Assert.Throws<DataVerificationPersistenceException>(() => esw.Write(MockEventStreams.First));
                }
            }
        }

        [Test]
        public void Given_first_set_when_artificially_truncated_and_appended_to_with_second_set_then_it_will_throw_on_write() {
            using (var ms = new MemoryStream()) {
                ResourceProvider.AppendTo(ms, "First.e", 7 /* magic sauce to truncate by an arbitrary 7 bytes */);
                
                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    Assert.Throws<DataVerificationPersistenceException>(() => esw.Write(MockEventStreams.Second));
                }
            }
        }
    }
}
