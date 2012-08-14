using System;
using System.Collections.Generic;
using System.IO;

namespace EventStreams.Persistence.Serialization {
    using Core;
    using Domain;
    using Snapshots;

    class SerializerTests : IPerformanceTestSuite {
        private readonly Serializer _serializer = new Serializer();
        private readonly MemoryStream _ms = new MemoryStream(64);

        public IEnumerable<Action> GetTests() {
            yield return Serialize;
            yield return Deserialize;
        }

        public int Repeat {
            get { return 200000; }
        }

        public SerializerTests() {
            var tmp = new BankAccountState { Balance = 111.95m };
            _serializer.Serialize(_ms, tmp);
        }

        private void Serialize() {
            var tmp = new BankAccountState { Balance = 111.95m };
            using (var ms = new MemoryStream(64))
                _serializer.Serialize(ms, tmp);
        }

        private void Deserialize() {
            _serializer.Deserialize<BankAccountState>(_ms);
        }
    }
}
