using System;

namespace EventStreams.Domain {
    using Core.Domain;
    using Events.BankAccount;

    public class BankAccount : IAggregateRoot {
        private readonly Memento<BankAccountState> _memento;
        private readonly CommandHandler<BankAccount> _commandHandler;
        public decimal Balance { get { return _memento.State.Balance; } }

        public BankAccount()
            : this(null) { }

        public BankAccount(Memento<BankAccountState> memento) {
            _memento = memento ?? new Memento<BankAccountState>();
            _commandHandler = new CommandHandler<BankAccount>(this);
        }

        public void Credit(decimal value) {
            _commandHandler.OnNext(new Credited(value));
        }

        public void Debit(decimal value) {
            _commandHandler.OnNext(new Debited(value));
        }

        public void Purchase(decimal value, string name) {
            _commandHandler.OnNext(new MadePurchase(value, name));
        }

        public void DepositPayeSalary(decimal value, string source) {
            _commandHandler.OnNext(new PayeSalaryDeposited(value, source));
        }

        protected void Handle(Credited args) {
            _memento.State.Balance += args.Value;
        }

        protected void Handle(Debited args) {
            _memento.State.Balance -= args.Value;
        }

        protected void Handle(MadePurchase args) {
            _memento.State.Balance -= args.Value;
        }

        protected void Handle(PayeSalaryDeposited args) {
            _memento.State.Balance += args.Value;
        }

        Guid IAggregateRoot.Identity {
            get { return _memento.Identity; }
        }

        object IAggregateRoot.Memento {
            get { return _memento.State; }
        }

        IDisposable IObservable<EventArgs>.Subscribe(IObserver<EventArgs> observer) {
            return _commandHandler.Subscribe(observer);
        }

        public void Dispose() {
            _commandHandler.OnCompleted();
        }
    }
}
