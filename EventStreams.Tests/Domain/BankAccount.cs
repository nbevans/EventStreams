using System;

namespace EventStreams.Domain {
    using Core;
    using Core.Domain;
    using Extensions;
    using Events.BankAccount;

    public class BankAccount : IObservable<MadePurchase>, IObserver<IStreamedEvent> {
        private readonly BankAccountState _state;
        private readonly IObserver<IStreamedEvent> _handler;

        public decimal Balance { get { return _state.Balance; } }

        public BankAccount()
            : this(null) {

            _handler = new ConventionEventHandler<BankAccount>(this);

            //_handler =
            //    new AggregateRootEventHandler<BankAccount>(this)
            //        .Bind<Credited>(e => _state.Balance += e.Args<Credited>().Value)
            //        .Bind<Debited>(e => _state.Balance -= e.Args<Debited>().Value)
            //        .Bind<MadePurchase>(e => _state.Balance -= e.Args<MadePurchase>().Value)
            //        .Bind<PayeSalaryDeposited>(e => _state.Balance += e.Args<PayeSalaryDeposited>().Value);
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

        void IObserver<IStreamedEvent>.OnNext(IStreamedEvent value) {
            _handler.OnNext(value);
        }

        void IObserver<IStreamedEvent>.OnError(Exception error) {
            _handler.OnError(error);
        }

        void IObserver<IStreamedEvent>.OnCompleted() {
            _handler.OnCompleted();
        }
    }
}
