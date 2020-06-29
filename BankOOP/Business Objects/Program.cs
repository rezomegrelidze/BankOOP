using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BankOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();
            var bank = new Bank("Leumi","Dizengoff 123, Tel-Aviv");
            for(int i = 0;i < 100;i++)
                bank.AddNewCustomer(new Customer(123 + i,"Raz" + i,"111000333"));

            for (int i = 123; i < 123 + 100; i++)
            {
                var customer = bank.GetCustomerById(i);
                bank.OpenNewAccount(new Account(customer, rand.Next(3000, 100000)), customer);
            }
        }
    }
}
