using System;
using System.IO;
using System.Text;
using Moq;

using NUnit.Framework;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;
    using Domain.Events.BankAccount;

    [TestFixture]
    internal class EventStreamWriterTests {

        private readonly IStreamedEvent[] _firstEvents = new[] {
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("20F65C10-D7DE-43B0-A527-4CCC43496BFE") && f.Timestamp == DateTime.MinValue && f.Arguments == new PayeSalaryDeposited(100, "Acme Corp")),
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("685F85DA-07AC-4EB3-B7F5-F52BCD543E84") && f.Timestamp == DateTime.MinValue && f.Arguments == new MadePurchase(45, "Wine"))
        };

        private readonly IStreamedEvent[] _secondEvents = new[] {
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("1012E630-8325-47D4-9393-DCD7F5940E09") && f.Timestamp == DateTime.MinValue && f.Arguments == new PayeSalaryDeposited(150, "Acme Corp")),
            Mock.Of<IStreamedEvent>(f => f.Id == new Guid("E19772BA-6DAE-408E-9F09-8561889C8154") && f.Timestamp == DateTime.MinValue && f.Arguments == new MadePurchase(25, "Cheese"))
        };

        [Test]
        public void Given_first_set_when_read_back_then_output_is_as_expected() {
            using (var ms = new MemoryStream()) {
                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    esw.Write(_firstEvents);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms)) {
                        Assert.That(sr.ReadToEnd(), Is.EqualTo(Strings.FirstEvents));
                    }
                }
            }
        }

        [Test]
        public void Given_first_set_when_appended_to_with_second_set_then_hashes_continue_from_previous_hash() {
            using (var ms = new MemoryStream()) {
                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    esw.Write(_firstEvents);
                    esw.Write(_secondEvents);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms)) {
                        Assert.That(sr.ReadToEnd(), Is.EqualTo(Strings.FirstAndSecondEvents));
                    }
                }
            }
        }

        [Test]
        public void Given_a_hash_seeded_stream_when_appended_to_with_first_set_then_hashing_continues_from_seed_hash() {
            using (var ms = new MemoryStream()) {
                var hashSeedLine = Encoding.UTF8.GetBytes("Hash:  YXNkYXNkYXNkYWFzZGFzZGFzZGE=\r\n\r\n");
                ms.Write(hashSeedLine, 0, hashSeedLine.Length);

                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    esw.Write(_firstEvents);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms)) {
                        Assert.That(sr.ReadToEnd(), Is.EqualTo(Strings.FirstEvents_WithHashSeed));
                    }
                }
            }
        }

        [Test]
        public void Given_a_malformed_hash_seeded_stream_when_appended_to_with_first_set_then_it_will_throw() {
            using (var ms = new MemoryStream()) {
                var hashSeedLine = Encoding.UTF8.GetBytes("HaSH:  YXNkYXNkYXNkYWFzZGFzZGFzZGE=\r\n\r\n");
                ms.Write(hashSeedLine, 0, hashSeedLine.Length);

                using (var esw = new EventStreamWriter(ms, new NullEventWriter())) {
                    Assert.Throws<InvalidOperationException>(() => esw.Write(_firstEvents));
                }
            }
        }
    }
}
