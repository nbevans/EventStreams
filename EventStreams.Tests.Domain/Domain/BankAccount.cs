using System;

namespace EventStreams.Domain {
    using Core.Domain;
    using Events.BankAccount;

    public class BankAccount : IObservable<EventArgs>, IDisposable {
        private readonly BankAccountState _memento;
        private readonly EventDispatcher<BankAccount> _eventDispatcher;
        
        public Guid Identity { get { return _memento.Identity; }}
        public decimal Balance { get { return _memento.Balance; } }

        public BankAccount()
            : this(null) { }

        public BankAccount(BankAccountState memento) {
            _memento = memento ?? new BankAccountState();
            _eventDispatcher = new EventDispatcher<BankAccount>(this);
        }

        public void Credit(decimal value) {
            _eventDispatcher.Dispatch(new Credited(value));
        }

        public void Debit(decimal value) {
            _eventDispatcher.Dispatch(new Debited(value));
        }

        public void Purchase(decimal value, string name) {
            _eventDispatcher.Dispatch(new MadePurchase(value, name));
        }

        public void DepositPayeSalary(decimal value, string source) {
            _eventDispatcher.Dispatch(new PayeSalaryDeposited(value, source));
        }

        protected void Apply(Credited args) {
            _memento.Balance += args.Value;
        }

        protected void Apply(Debited args) {
            _memento.Balance -= args.Value;
        }

        protected void Apply(MadePurchase args) {
            _memento.Balance -= args.Value;
        }

        protected void Apply(PayeSalaryDeposited args) {
            _memento.Balance += args.Value;
        }

        IDisposable IObservable<EventArgs>.Subscribe(IObserver<EventArgs> observer) {
            return _eventDispatcher.Subscribe(observer);
        }

        public void Dispose() {
            _eventDispatcher.Dispose();
        }
    }
}
