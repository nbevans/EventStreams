using System;
using System.Collections.Generic;

namespace EventStreams.Projection {
    using Core;
    using Domain.Accounting;
    using Domain.Accounting.Events;
    using EventHandling;

    class ProjectorTests : IPerformanceTestSuite {

        private readonly DefaultProjector _projector = new DefaultProjector();

        private readonly Func<BankAccount, EventHandler> _defaultEventHandlerFactory =
            model => new ConventionEventHandler(model);

        private readonly StreamedEvent[] _events100 = new[] {
            new PayeSalaryDeposited(100, "Acme Corp").ToStreamedEvent(),
            new PayeSalaryDeposited(50, "Acme Corp").ToStreamedEvent(),
            new MadePurchase(5, "Cheese").ToStreamedEvent(),
            new MadePurchase(45, "Wine").ToStreamedEvent()
        };

        public IEnumerable<Action> GetTests() {
            yield return ProjectSequenceOfFourEvents;
        }

        public int Repeat {
            get { return 1000000; }
        }

        private void ProjectSequenceOfFourEvents() {
            _projector.Project(_events100, _defaultEventHandlerFactory);
        }
    }
}
