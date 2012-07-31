using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using TestDomain;
    using TestDomain.Events.BankAccount;

    class ProjectorTests : IPerformanceTestSuite {

        private readonly Projector _projector = new Projector();
        private readonly StreamedEventArgs[] _events100 = new StreamedEventArgs[] {
            new PayeSalaryDeposited(100, "Acme Corp"),
            new PayeSalaryDeposited(50, "Acme Corp"),
            new MadePurchase(5, "Cheese"),
            new MadePurchase(45, "Wine")
        };

        public ProjectorTests() {
            _projector.Cache<BankAccount>();
        }

        public IEnumerable<Action> GetTests() {
            yield return DoIt;
        }

        public int Repeat
        {
            get { return 1000000; }
        }

        private void DoIt() {
            _projector.Project<BankAccount>(_events100);
        }
    }
}
