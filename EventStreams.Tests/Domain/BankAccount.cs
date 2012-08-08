using System;
using System.Reactive.Subjects;

namespace EventStreams.Domain
{
    using Core;
    using Events.BankAccount;

    public class BankAccount : IObservable<MadePurchase>, IObserver<MadePurchase> {
        private readonly BankAccountState _state;
        private Subject<BankAccount> _subject;

        public decimal Balance { get { return _state.Balance; } }

        public BankAccount()
            : this(null) { }

        public BankAccount(BankAccountState state) {
            _state = state ?? new BankAccountState();

            Credited += (e, p) => _state.Balance += e.Value;
            Debited += (e, p) => _state.Balance -= e.Value;
            MadePurchase += (e, p) => Debited(e, p);
            PayeSalaryDeposited += (e, p) => Credited(e, p);
        }

        public event StreamedEventHandler<Credited> Credited;

        public event StreamedEventHandler<Debited> Debited;

        public event StreamedEventHandler<MadePurchase> MadePurchase;

        public event StreamedEventHandler<PayeSalaryDeposited> PayeSalaryDeposited;

        public void Credit(decimal value) {
            new Credited(value).Invoke(Credited);
        }

        public void Debit(decimal value) {
            new Debited(value).Invoke(Debited);
        }

        public void Purchase(decimal value, string name) {
            new MadePurchase(value, name).Invoke(MadePurchase);
        }

        public void DepositPayeSalary(decimal value, string source) {
            new PayeSalaryDeposited(value, source).Invoke(PayeSalaryDeposited);
        }

        public IDisposable Subscribe(IObserver<MadePurchase> observer) {
            throw new NotImplementedException();
        }

        public void OnNext(MadePurchase value) {
            throw new NotImplementedException();
        }

        public void OnError(Exception error) {
            throw new NotImplementedException();
        }

        public void OnCompleted() {
            throw new NotImplementedException();
        }
    }
}
