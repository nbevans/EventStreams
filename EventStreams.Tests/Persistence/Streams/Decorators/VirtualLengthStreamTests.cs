using System;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace EventStreams.Persistence.Streams.Decorators {

    [TestFixture]
    public class VirtualLengthStreamTests {

        private readonly byte[] _data = Encoding.UTF8.GetBytes("{ }_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text_trailing-text");
        private readonly string _expected = "{ }";

        [Test]
        public void Given_some_data_when_length_limited_to_3_bytes_and_read_to_end_with_default_buffering_then_read_data_is_as_expected_and_position_has_not_exceeeded_limit() {
            using (var ms = new MemoryStream()) {
                ms.Write(_data, 0, _data.Length);
                ms.Position = 0;

                using (var lls = ms.VirtualLength(3))
                using (var sr = new StreamReader(lls)) {
                    var result = sr.ReadToEnd();
                    Assert.AreEqual(_expected, result);
                    Assert.That(ms.Position, Is.EqualTo(3));
                }

                ms.Position = 0;

                using (var lls = ms.VirtualLength(3)) {
                    Assert.That(lls.ReadByte() == '{');
                    Assert.That(lls.ReadByte() == ' ');
                    Assert.That(lls.ReadByte() == '}');
                    Assert.That(ms.Position, Is.EqualTo(3));
                }
            }
        }

        [Test]
        public void Given_some_data_when_length_limited_to_3_bytes_and_read_first_3_bytes_individually_with_minimal_buffering_then_read_data_is_as_expected_and_position_has_not_exceeeded_limit() {
            using (var ms = new MemoryStream()) {
                ms.Write(_data, 0, _data.Length);
                ms.Position = 0;

                using (var lls = ms.VirtualLength(3))
                using (var sr = new StreamReader(lls, Encoding.UTF8, false, 1)) {
                    Assert.That(sr.Read() == _expected[0]);
                    Assert.That(sr.Read() == _expected[1]);
                    Assert.That(sr.Read() == _expected[2]);
                    Assert.That(ms.Position, Is.EqualTo(3));
                }
            }
        }

        [Test]
        public void Given_some_data_when_length_limited_to_3_bytes_and_read_to_end_with_zero_buffering_then_read_data_is_as_expected_and_position_has_not_exceeeded_limit() {
            using (var ms = new MemoryStream()) {
                ms.Write(_data, 0, _data.Length);
                ms.Position = 0;

                using (var lls = ms.VirtualLength(3)) {
                    Assert.That(lls.ReadByte() == _expected[0]);
                    Assert.That(lls.ReadByte() == _expected[1]);
                    Assert.That(lls.ReadByte() == _expected[2]);
                    Assert.That(ms.Position, Is.EqualTo(3));
                }
            }
        }

        [Test]
        public void Given_a_length_limited_stream_when_doing_something_other_than_reading_then_expect_it_to_throw() {
            using (var ms = new MemoryStream())
            using (var lls = ms.VirtualLength(3)) {
                Assert.Throws<NotSupportedException>(() => lls.Write(new byte[5], 0, 5));
                Assert.Throws<NotSupportedException>(() => lls.SetLength(5));
            }
        }
    }
}
