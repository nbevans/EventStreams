using System;
using EventStreams.Core;
using EventStreams.TestDomain;
using NUnit.Framework;

namespace EventStreams.Projection
{
    using TestDomain.Events;

    [TestFixture]
    public class SimpleProjectorTests
    {
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
        }

        [Test]
        public void Project()
        {
            var events = new EventBase<BankAccount>[]
                             {
                                 new SalaryDeposited(100),
                                 new SalaryDeposited(50),
                                 new CheeseProductPurchased(5),
                                 new WineProductPurchased(45)
                             };

            var obj = new Projector().Project(events);
            Assert.That(obj.Balance == 100);
        }
    }
}
