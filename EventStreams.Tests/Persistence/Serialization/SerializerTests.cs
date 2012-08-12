using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

using NUnit.Framework;

namespace EventStreams.Persistence {
    using Serialization;

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

        [Test]
        public void Expect_that_nested_reference_types_can_be_serialized_and_deserialized_accurately() {
            var nestedObjIn1 = new TestState {
                A = decimal.MaxValue,
                B = "Hello World!",
                C = DateTime.MaxValue,
                D = long.MaxValue,
                E = new byte[] { 1, 2, 3 },
                F = true
            };

            var nestedObjIn2 = new TestState {
                A = decimal.MinValue,
                B = "Hello Galaxy!",
                C = DateTime.MinValue,
                D = long.MinValue,
                E = new byte[] { 3, 2, 1 },
                F = true
            };

            var containerObjIn = new TestContainerState {
                A = nestedObjIn1,
                B = nestedObjIn2
            };

            var ms = new MemoryStream(128);
            var serializer = new Serializer();

            serializer.Serialize(ms, containerObjIn);
            ms.Position = 0;
            var objContainerOut = serializer.Deserialize<TestContainerState>(ms);

            Assert.That(objContainerOut.A.A == containerObjIn.A.A);
            Assert.That(objContainerOut.A.B == containerObjIn.A.B);
            Assert.That(objContainerOut.A.C == containerObjIn.A.C);
            Assert.That(objContainerOut.A.D == containerObjIn.A.D);
            Assert.That(objContainerOut.A.E.SequenceEqual(containerObjIn.A.E));
            Assert.That(objContainerOut.A.F == containerObjIn.A.F);

            Assert.That(objContainerOut.B.A == containerObjIn.B.A);
            Assert.That(objContainerOut.B.B == containerObjIn.B.B);
            Assert.That(objContainerOut.B.C == containerObjIn.B.C);
            Assert.That(objContainerOut.B.D == containerObjIn.B.D);
            Assert.That(objContainerOut.B.E.SequenceEqual(containerObjIn.B.E));
            Assert.That(objContainerOut.B.F == containerObjIn.B.F);
        }

        [DataContract]
        public class TestContainerState {

            [DataMember(Order = 1)]
            public TestState A { get; set; }

            [DataMember(Order = 2)]
            public TestState B { get; set; }
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
