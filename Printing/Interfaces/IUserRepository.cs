using Printing.Models;

namespace Printing.Interfaces
{
    public interface IUserRepository 
    {
        Task<string> SearchByName(string name);
    }
}
