namespace BankOOP
{
    public interface IBank
    {
        string Name { get; }
        string Address { get; }
        int CustomerCount { get; }
    }
}