using System;
using System.Collections.Generic;
using System.IO;

namespace EventStreams.Serialization
{
    using Core;
    using TestDomain;

    class SerializerTests : IPerformanceTestSuite
    {
        private readonly Serializer _serializer = new Serializer();
        private readonly MemoryStream ms = new MemoryStream(64);

        public IEnumerable<Action> GetTests()
        {
            yield return Serialize;
            yield return Deserialize;
        }

        public int Repeat
        {
            get { return 200000; }
        }

        public SerializerTests()
        {
            var tmp = new BankAccountState { Balance = 111.95m, Foo = "foobar", Dt = DateTime.UtcNow };
            _serializer.Serialize(ms, tmp);
        }

        private void Serialize()
        {
            var tmp = new BankAccountState {Balance = 111.95m, Foo = "foobar", Dt = DateTime.UtcNow};
            using (var ms = new MemoryStream(64))
                _serializer.Serialize(ms, tmp);
        }

        private void Deserialize()
        {
            _serializer.Deserialize<BankAccountState>(ms);
        }
    }
}
