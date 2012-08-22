using System;
using System.Linq;
using System.IO;

using NUnit.Framework;

namespace EventStreams.Persistence {
    using Serialization.Events;
    using Resources;

    [TestFixture]
    internal class EventStreamReaderTests {

        [Test]
        public void Given_first_set_when_read_back_then_output_is_as_expected() {
            using (var ms = new MemoryStream()) {
                ResourceProvider.AppendTo(ms, "First.e");
                ms.Position = 0;

                using (var esr = new EventStreamReader(ms, new NullEventReader())) {
                    var items = esr.Read().ToArray();
                    var first = items.ElementAt(0);
                    var second = items.ElementAt(1);


                    //Assert.That(sr.ReadToEnd(), Is.EqualTo(Strings.FirstEvents));
                }
            }
        }
    }
}
