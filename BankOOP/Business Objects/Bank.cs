using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BankOOP.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankOOP
{
    public class Bank : IBank
    {
        [JsonProperty("Name")]
        public string Name { get; private set; }
        [JsonProperty("Address")]
        public string Address { get; private set; }
        public int CustomerCount => customers.Count;

        [JsonProperty("Accounts")]
        private List<Account> accounts;

        [JsonProperty("Customers")]
        private List<Customer> customers;

        private Dictionary<int, Customer> customerIDToCustomer;
        private Dictionary<int, Customer> customerNumberToCustomer;
        private Dictionary<Customer, List<Account>> customerToAccounts;

        [JsonProperty("TotalMoneyInBank")]
        private decimal totalMoneyInBank;

        [JsonProperty("Profits")]
        private decimal profits;

        public Bank(string bankName,string address)
        {
            Name = bankName;
            Address = address;
            accounts = new List<Account>();
            customers = new List<Customer>();
            customerIDToCustomer = new Dictionary<int, Customer>();
            customerNumberToCustomer = new Dictionary<int, Customer>();
            customerToAccounts = new Dictionary<Customer, List<Account>>();
        }

        public Customer GetCustomerById(int customerId)
        {
            if (!customerIDToCustomer.TryGetValue(customerId, out var customer)) throw new CustomerNotFoundException();
            return customer;
        }

        public Customer GetCustomerByNumber(int customerNumber)
        {
            if (!customerNumberToCustomer.TryGetValue(customerNumber, out var customer)) throw new CustomerNotFoundException();
            return customer;
        }

        public Account GetAccountByNumber(int accountNumber)
        {
            var result = accounts.SingleOrDefault(account => account.AccountNumber == accountNumber);
            if(result == null) throw new AccountNotFoundException();
            return result;
        }

        public List<Account> GetAccountsByCustomer(Customer customer)
        {
            return accounts.Where(account => account.AccountOwner == customer).ToList();
        }

        public void AddNewCustomer(Customer customer)
        {
            if(customers.Contains(customer)) throw new CustomerAlreadyExistException();
            customers.Add(customer);
            customerIDToCustomer[customer.CustomerID] = customer;
            customerNumberToCustomer[customer.CustomerNumber] = customer;
        }

        public void OpenNewAccount(Account account, Customer customer)
        {
            accounts.Add(account);
            if(customerToAccounts.ContainsKey(customer)) 
                customerToAccounts[customer].Add(account);
            else 
                customerToAccounts[customer] = new List<Account>();
        }

        public decimal Deposit(Account account, decimal amount)
        {
            totalMoneyInBank += amount;
            account.Add(amount);
            return account.Balance;
        }

        public decimal Withdraw(Account account, decimal amount)
        {
            if (Math.Abs(account.Balance - amount) < account.MaxMinusAllowed) throw new BalanceException();
            totalMoneyInBank -= amount;
            account.Subtract(amount);
            return account.Balance;
        }

        public decimal GetCustomerTotalBalance(Account account) => customerToAccounts[account.AccountOwner].Sum(a => a.Balance);

        public void CloseAccount(Account account, Customer customer)
        {
            accounts.Remove(account);
            customerToAccounts[customer].Remove(account);
        }

        public void ChargeAnnualCommission(decimal percentage)
        {
            foreach (var account in accounts)
            {
                var charge = account.Balance * percentage;
                if (account.Balance < 0) charge *= 2;
                account.Subtract(charge);
                profits += charge;
            }
        }

        public void JoinAccounts(Account a, Account b)
        {
            if (a.AccountOwner != b.AccountOwner) throw new NotSameCustomerException();

            OpenNewAccount(a + b,a.AccountOwner);
            CloseAccount(a,a.AccountOwner);
            CloseAccount(b,b.AccountOwner);
        }

        public string Save()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void SaveToFile(string destinationPath)
        {
            File.WriteAllText(destinationPath,Save());
        }

        public static Bank Load(string json)
        {
            var bankState = JsonConvert.DeserializeObject<Bank>(json);
            bankState.accounts = PopulateAccounts(JObject.Parse(json)["Accounts"]).ToList();
            var bank = new Bank(bankState.Name,bankState.Address);

            foreach(var customer in bankState.customers)
                bank.AddNewCustomer(customer);

            foreach(var account in bankState.accounts)
                bank.OpenNewAccount(account,account.AccountOwner);

            return bank;
        }

        private static IEnumerable<Account> PopulateAccounts(JToken accountsJson)
        {
            foreach (var account in accountsJson)
            {
                var resultAccount = JsonConvert.DeserializeObject<Account>(account.ToString());
                resultAccount.AccountOwner = JsonConvert.DeserializeObject<Customer>(account["AccountOwner"].ToString());
                yield return resultAccount;
            }
        }

        public static Bank LoadFromFile(string jsonFilePath)
        {
            return Load(File.ReadAllText(jsonFilePath));
        }
    }
}