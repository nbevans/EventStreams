using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

using NUnit.Framework;

namespace EventStreams.Serialization {

    [TestFixture]
    class SerializerTests {

        [Test]
        public void Expect_that_a_serialized_object_can_be_deserialized_accurately() {
            var objIn = new TestState {
                A = decimal.MaxValue,
                B = "Hello World!",
                C = DateTime.MaxValue,
                D = long.MaxValue,
                E = new byte[] { 1, 2, 3 },
                F = true
            };

            var ms = new MemoryStream(128);
            var serializer = new Serializer();

            serializer.Serialize(ms, objIn);
            ms.Position = 0;
            var objOut = serializer.Deserialize<TestState>(ms);

            Assert.That(objOut.A == objIn.A);
            Assert.That(objOut.B == objIn.B);
            Assert.That(objOut.C == objIn.C);
            Assert.That(objOut.D == objIn.D);
            Assert.That(objOut.E.SequenceEqual(objIn.E));
            Assert.That(objOut.F == objIn.F);
        }

        [DataContract]
        public class TestState {

            [DataMember(Order = 1)]
            public decimal A { get; set; }

            [DataMember(Order = 2)]
            public string B { get; set; }

            [DataMember(Order = 3)]
            public DateTime C { get; set; }

            [DataMember(Order = 4)]
            public long D { get; set; }

            [DataMember(Order = 5)]
            public byte[] E { get; set; }

            [DataMember(Order = 6)]
            public bool F { get; set; }
        }
    }
}
