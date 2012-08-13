using System;
using System.Linq;
using System.Xml;
using System.IO;

using NUnit.Framework;

namespace EventStreams.Persistence {
    using Core;
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
                using (var esw = new EventStreamWriter(ms)) {
                    esw.Write(_events100);

                    ms.Position = 0;
                    using (var sr = new StreamReader(ms)) {
                        TestLine(sr, "Id: " + _events100.ElementAt(0).Id);
                        TestLine(sr, "Timestamp: " + _events100.ElementAt(0).Timestamp.ToString("O"));
                        ReadXmlDocument(sr);
                    }
                }
            }
        }

        private void TestLine(StreamReader sr, string expected) {
            var line = sr.ReadLine();
            Assert.That(line == expected);
        }

        private void ReadXmlDocument(StreamReader sr) {
            using (var xr = XmlReader.Create(sr, new XmlReaderSettings { CloseInput = false, ConformanceLevel = ConformanceLevel.Document })) {
                while (xr.ReadState != ReadState.EndOfFile)
                    xr.Read();
            }
        }
    }
}
