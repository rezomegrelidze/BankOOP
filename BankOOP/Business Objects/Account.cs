using System;
using BankOOP.Exceptions;
using Newtonsoft.Json;

namespace BankOOP
{
    public class Account
    {
        private static int numberOfAcc;
        private readonly int accountNumber;
        private decimal maxMinusAllowed;
        public int AccountNumber => accountNumber;
        public decimal Balance { get; private set; }
        public Customer AccountOwner { get; internal set; }

        public decimal MaxMinusAllowed => maxMinusAllowed;

        public Account(Customer customer,decimal monthlyIncome)
        {
            AccountOwner = customer;
            accountNumber = numberOfAcc++;
            maxMinusAllowed = monthlyIncome * 3m;
        }

        public void Add(decimal amount) => Balance += amount;
        public void Subtract(decimal amount) => Balance -= amount;
        public static bool operator ==(Account a, Account b)
        {
            if (a != null && b != null)
            {
                return a.accountNumber == b.accountNumber;
            }
            return false;
        }

        public static bool operator !=(Account a, Account b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var otherAccount = obj as Account;
            if (!(otherAccount is null)) return accountNumber == otherAccount.accountNumber;
            return false;
        }

        public override int GetHashCode()
        {
            return accountNumber.GetHashCode();
        }

        public static Account operator +(Account a,Account b)
        {
            if(a.AccountOwner != b.AccountOwner) throw new NotSameCustomerException();

            return new Account(a.AccountOwner,a.MaxMinusAllowed / 3m)
            {
                Balance = a.Balance + b.Balance
            };
        }
    }
}