using System;

namespace EventStreams.Domain {
    using Core;
    using Core.Domain;
    using Extensions;
    using Events.BankAccount;

    public class BankAccount : IObservable<MadePurchase>, IObserver<EventArgs> {
        private readonly BankAccountState _state;
        private readonly IObserver<EventArgs> _handler;

        public decimal Balance { get { return _state.Balance; } }

        public BankAccount()
            : this(null) {

            _handler = new ConventionEventHandler<BankAccount>(this);

            _handler =
                new DelegatedEventHandler<BankAccount>(this)
                    .Bind<Credited>(e => _state.Balance += e.Assume<Credited>().Value)
                    .Bind<Debited>(e => _state.Balance -= e.Assume<Debited>().Value)
                    .Bind<MadePurchase>(e => _state.Balance -= e.Assume<MadePurchase>().Value)
                    .Bind<PayeSalaryDeposited>(e => _state.Balance += e.Assume<PayeSalaryDeposited>().Value);
        }

        public BankAccount(BankAccountState state) {
            _state = state ?? new BankAccountState();
        }

        public void Credit(decimal value) {
        }

        public void Debit(decimal value) {
        }

        public void Purchase(decimal value, string name) {
        }

        public void DepositPayeSalary(decimal value, string source) {
        }

        protected void Handle(Credited args) {
            _state.Balance += args.Value;
        }

        protected void Handle(Debited args) {
            _state.Balance -= args.Value;
        }

        protected void Handle(MadePurchase args) {
            _state.Balance -= args.Value;
        }

        protected void Handle(PayeSalaryDeposited args) {
            _state.Balance += args.Value;
        }

        public IDisposable Subscribe(IObserver<MadePurchase> observer) {
            return Disposable.Empty;
        }

        void IObserver<EventArgs>.OnNext(EventArgs value) {
            _handler.OnNext(value);
        }

        void IObserver<EventArgs>.OnError(Exception error) {
            _handler.OnError(error);
        }

        void IObserver<EventArgs>.OnCompleted() {
            _handler.OnCompleted();
        }
    }
}
