using System;

namespace EventStreams.TestDomain
{
    class BankAccount
    {
        public decimal Balance { get; private set; }

        public BankAccount Credit(decimal value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException("value");
            Balance += value;
            return this;
        }

        public BankAccount Debit(decimal value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException("value");
            Balance -= value;
            return this;
        }
    }
}
