using Microsoft.EntityFrameworkCore;
using Printing.Data;
using Printing.Interfaces;
using Printing.Models;
using System.Numerics;

namespace Printing.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly PhotoContext _db;
        public UserRepository(PhotoContext db)
        {
            _db = db;
        }

        public async Task<string> SearchByName(string name)
        {
            try
            {
                var users = await _db.Users.Where(u => u.FullName.Contains(name)).ToListAsync();
                string result = Newtonsoft.Json.JsonConvert.SerializeObject(users);
                return result;
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}
