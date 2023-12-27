using Microsoft.EntityFrameworkCore;
using Printing.Data;
using Printing.Interfaces;

namespace Printing.Service
{
    public class CustomerApi : ICustomerApi
    {
        private readonly PhotoContext _db;

        public CustomerApi(PhotoContext db)
        {
            _db = db;
        }
        public async Task<string> SearchUserByPhone(string Phone)
        {
            try
            {
                // Assume you have a DbSet<Customer> in your PhotoContext
                var user = await _db.Users.Where(c => c.Phone == Phone).ToListAsync();
                // Convert the list of customers to a string representation, for example using JSON serialization
                string result = Newtonsoft.Json.JsonConvert.SerializeObject(user);
                return result;
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}
