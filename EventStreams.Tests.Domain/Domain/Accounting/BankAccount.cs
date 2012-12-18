using System;
using System.Runtime.Serialization;
using System.Reactive.Subjects;

namespace EventStreams.Domain.Accounting {
    using Events;

    public class BankAccount : IObservable<EventArgs>, IDisposable {
        private readonly State _memento;
        private readonly Subject<EventArgs> _subject;
        
        public Guid Identity { get { return _memento.Identity; }}
        public decimal Balance { get { return _memento.Balance; } }

        public BankAccount()
            : this(null) { }

        public BankAccount(State memento) {
            _memento = memento ?? new State();
            _subject = new Subject<EventArgs>();
        }

        public void Credit(decimal value) {
            _subject.OnNext(new Credited(value));
        }

        public void Debit(decimal value) {
            _subject.OnNext(new Debited(value));
        }

        public void Purchase(decimal value, string name) {
            _subject.OnNext(new MadePurchase(value, name));
        }

        public void DepositPayeSalary(decimal value, string source) {
            _subject.OnNext(new PayeSalaryDeposited(value, source));
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
            return _subject.Subscribe(observer);
        }

        public void Dispose() {
            _subject.Dispose();
        }

        [DataContract]
        public class State {

            [IgnoreDataMember]
            public Guid Identity { get; private set; }

            [DataMember(Order = 1)]
            public decimal Balance { get; set; }

            public State()
                : this(Guid.NewGuid()) { }

            public State(Guid identity) {
                Identity = identity;
            }
        }
    }
}
