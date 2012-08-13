using System;
using System.IO;

using NUnit.Framework;

namespace EventStreams.Persistence {
    using Core;
    using Serialization.Events;
    using Domain.Events.BankAccount;

    [TestFixture]
    internal class EventStreamWriterTests {

        private readonly StreamedEvent[] _events100 = new[] {
            new PayeSalaryDeposited(100, "Acme Corp").ToStreamedEvent(),
            new PayeSalaryDeposited(50, "Acme Corp").ToStreamedEvent(),
            new MadePurchase(5, "Cheese").ToStreamedEvent(),
            new MadePurchase(45, "Wine").ToStreamedEvent()
        };

        [Test]
        public void Given_events100_when_written_to_memory_stream_then_output_is_as_expected() {
            using (var ms = new MemoryStream()) {
                using (var esw = new EventStreamWriter(ms, new BlobEventWriter())) {
                    // When
                    esw.Write(_events100);

                    // Then
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms)) {
                        foreach (var se in _events100) {
                            TestLine(sr, "Id:  " + se.Id);
                            TestLine(sr, "Timestamp:  " + se.Timestamp.ToString("O"));
                            TestLine(sr, "Type:  " + se.Arguments.GetType().AssemblyQualifiedName);
                            TestLine(sr, "{ }");
                            TestLine(sr, "");
                        }
                    }
                }
            }
        }

        private void TestLine(StreamReader sr, string expected) {
            var line = sr.ReadLine();
            Assert.That(line, Is.EqualTo(expected));
        }

        private sealed class BlobEventWriter : IEventWriter {
            public void Write(Stream innerStream, EventArgs args) {
                innerStream.Write(new[] { (byte)'{', (byte)' ', (byte)'}' }, 0, 3);
            }
        }
    }
}
