using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStreams.TestDomain;
using NUnit.Framework;

namespace EventStreams.Serialization
{
    [TestFixture]
    class SerializerTests
    {
        [Test]
        public void foo()
        {
            var tmp = new BankAccountState {Balance = 111.95m, Foo = "foobar"};

            new Serializer().Serialize(tmp);
        }
    }
}
