namespace BankOOP
{
    public class Customer
    {
        private static int numberOfCust;
        private readonly int customerID;
        private readonly int customerNumber;
        public string Name { get; private set; }
        public string PhNumber { get; private set; }
        public int CustomerID => customerID;
        public int CustomerNumber => customerNumber;

        public Customer(int id,string name,string phone)
        {
            customerID = id;
            Name = name;
            PhNumber = phone;
            customerNumber = numberOfCust++;
        }

        public static bool operator ==(Customer a, Customer b)
        {
            if (a != null && b != null)
                return a.Equals(b);
            return false;
        }

        public static bool operator !=(Customer a, Customer b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            var otherCustomer = obj as Customer;
            if (!(otherCustomer is null)) return customerNumber == otherCustomer.customerNumber;
            return false;
        }

        public override int GetHashCode()
        {
            return customerNumber.GetHashCode();
        }
    }
}