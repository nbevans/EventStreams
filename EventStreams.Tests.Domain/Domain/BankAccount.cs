using System;

namespace EventStreams.Domain {
    using Core.Domain;
    using Events.BankAccount;

    public class BankAccount : IAggregateRoot {
        private readonly Memento<BankAccountState> _memento;
        private readonly CommandObservation<BankAccount> _commandObservation;
        public decimal Balance { get { return _memento.State.Balance; } }

        public BankAccount()
            : this(null) { }

        public BankAccount(Memento<BankAccountState> memento) {
            _memento = memento ?? new Memento<BankAccountState>();
            _commandObservation = new CommandObservation<BankAccount>(this);
        }

        public void Credit(decimal value) {
            _commandObservation.Apply(new Credited(value));
        }

        public void Debit(decimal value) {
            _commandObservation.Apply(new Debited(value));
        }

        public void Purchase(decimal value, string name) {
            _commandObservation.Apply(new MadePurchase(value, name));
        }

        public void DepositPayeSalary(decimal value, string source) {
            _commandObservation.Apply(new PayeSalaryDeposited(value, source));
        }

        protected void Apply(Credited args) {
            _memento.State.Balance += args.Value;
        }

        protected void Apply(Debited args) {
            _memento.State.Balance -= args.Value;
        }

        protected void Apply(MadePurchase args) {
            _memento.State.Balance -= args.Value;
        }

        protected void Apply(PayeSalaryDeposited args) {
            _memento.State.Balance += args.Value;
        }

        Guid IEventSourced.Identity {
            get { return _memento.Identity; }
        }

        object IEventSourced.Memento {
            get { return _memento.State; }
        }

        IDisposable IObservable<EventArgs>.Subscribe(IObserver<EventArgs> observer) {
            return _commandObservation.Subscribe(observer);
        }

        public void Dispose() {
            _commandObservation.Dispose();
        }
    }
}
