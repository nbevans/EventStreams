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
    internal class EventStreamReaderTests {

        [Test]
        public void Given_first_set_when_read_back_then_output_is_as_expected() {
            using (var ms = new MemoryStream()) {
                //var bytes = Encoding.UTF8.GetBytes(Strings.FirstEvents);
                //ms.Write(bytes, 0, bytes.Length);
                //ms.Position = 0;

                //using (var esr = new EventStreamReader(ms, new NullEventReader())) {
                //    esr.Read();
                //    //Assert.That(sr.ReadToEnd(), Is.EqualTo(Strings.FirstEvents));
                //}
            }
        }
    }
}
