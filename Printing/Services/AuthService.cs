using Printing.AppModels;
using Printing.Data;
using Printing.Interfaces;
using Printing.Models;

namespace Printing.Service
{
    public class AuthService : IAuthService
    {
        private readonly PhotoContext _db;
        private readonly IConfiguration _configuration;

        public AuthService(PhotoContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public Facilities AddFacilities(Facilities facilities)
        {
            throw new NotImplementedException();
        }

        public Role AddRole(Role role)
        {
            var addedRole = _db.Role.Add(role);
                _db.SaveChanges();
                return addedRole.Entity;
        }

        public Users AddUser(Users users)
        {
            try
            {
                // Kiểm tra xem username đã tồn tại chưa
                var existingUser = _db.Users.FirstOrDefault(u => u.UserName == users.UserName);
                if (existingUser != null)
                {
                    throw new Exception("Username already exists");
                }

                // Kiểm tra xem email đã tồn tại chưa
                var existingEmail = _db.Users.FirstOrDefault(u => u.Email == users.Email);
                if (existingEmail != null)
                {
                    throw new Exception("Email already exists");
                }

                // Hash mật khẩu trước khi thêm người dùng
                users.Password = users.HashPassword(users.Password);

                _db.Users.Add(users);
                _db.SaveChanges();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public bool AssignFacilitiesToUser(AddUserFacilities obj)
        {
            throw new NotImplementedException();
        }

        public bool AssignRoleToUser(AddUserRole obj)
        {
            try
            {
                var addRoles = new List<UserRole>();
                var user = _db.Users.SingleOrDefault(s => s.UserID == obj.UserId);
                if (user == null)
                    throw new Exception("User is not valid");
                foreach (int role in obj.RoleIds)
                {
                    var userRole = new UserRole();
                    userRole.RoleID = role;
                    userRole.UserID = user.UserID;
                    addRoles.Add(userRole);
                }
                _db.UserRole.AddRange(addRoles);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string Login(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }
    }
}
