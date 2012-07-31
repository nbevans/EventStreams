using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Domain;
    using Domain.Events.BankAccount;

    class ProjectorTests : IPerformanceTestSuite {

        private readonly Projector _projector = new Projector();
        private readonly StreamedEventArgs[] _events100 = new StreamedEventArgs[] {
            new PayeSalaryDeposited(100, "Acme Corp"),
            new PayeSalaryDeposited(50, "Acme Corp"),
            new MadePurchase(5, "Cheese"),
            new MadePurchase(45, "Wine")
        };

        public IEnumerable<Action> GetTests() {
            yield return ProjectSequenceOfFourEvents;
        }

        public int Repeat {
            get { return 1000000; }
        }

        private void ProjectSequenceOfFourEvents() {
            _projector.Project<BankAccount>(_events100);
        }
    }
}
