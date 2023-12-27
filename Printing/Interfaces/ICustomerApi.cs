namespace Printing.Interfaces
{
    public interface ICustomerApi
    {
        Task<string> SearchUserByPhone(string Phone);
    }
}
