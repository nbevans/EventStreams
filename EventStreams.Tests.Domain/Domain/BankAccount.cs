using System;

namespace EventStreams.Domain {
    using Core.Domain;
    using Events.BankAccount;

    public class BankAccount : IAggregateRoot {
        private readonly Memento<BankAccountState> _memento;
        private readonly EventHandler<BankAccount> _eventHandler;
        private readonly CommandHandler<BankAccount> _commandHandler;
        public decimal Balance { get { return _memento.State.Balance; } }

        public BankAccount()
            : this(null) { }

        public BankAccount(Memento<BankAccountState> memento) {
            _memento = memento ?? new Memento<BankAccountState>();
            _eventHandler = new ConventionEventHandler<BankAccount>(this);
            _commandHandler = new CommandHandler<BankAccount>(this);

            //_handler =
            //    new DelegatedEventHandler<BankAccount>(this)
            //        .Bind<Credited>(e => _state.Balance += e.Assume<Credited>().Value)
            //        .Bind<Debited>(e => _state.Balance -= e.Assume<Debited>().Value)
            //        .Bind<MadePurchase>(e => _state.Balance -= e.Assume<MadePurchase>().Value)
            //        .Bind<PayeSalaryDeposited>(e => _state.Balance += e.Assume<PayeSalaryDeposited>().Value);
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

        void IObserver<EventArgs>.OnNext(EventArgs value) {
            _eventHandler.OnNext(value);
        }

        void IObserver<EventArgs>.OnError(Exception error) {
            _eventHandler.OnError(error);
        }

        void IObserver<EventArgs>.OnCompleted() {
            _eventHandler.OnCompleted();
        }

        public void Dispose() {
            _commandHandler.OnCompleted();
        }
    }
}
