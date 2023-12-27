using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Printing.AppModels;
using Printing.Data;
using Printing.Interfaces;
using Printing.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PhotoContext _db;
        public AuthenController(PhotoContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            
        }   

            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginRequest loginRequest)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var user = _db.Users
                            .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                            .SingleOrDefault(s => s.UserName == loginRequest.Username);

                        if (user != null)
                        {
                            // Password verification logic
                            if (user.VerifyPassword(loginRequest.Password, user.Password))
                            {
                                var claims = new List<Claim> {
                            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                            new Claim("Id", user.UserID.ToString()),
                            new Claim("UserName", user.UserName)
                        };

                                // Add user roles to claims
                                if (user.UserRoles != null)
                                {
                                    foreach (var userRole in user.UserRoles)
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                                    }
                                }

                                // Query user facilities
                                var userFacilities = _db.UserFacilities
                                .Include(uf => uf.Facility)
                                .Where(uf => uf.UserID == user.UserID)
                                .Select(uf => uf.Facility)
                                .ToList();

                                // Get facility names
                                var facilityNames = userFacilities.Select(f => f.FacilityName).ToList();

                                // Add facility names to claims
                                foreach (var facilityName in facilityNames)
                                {
                                    claims.Add(new Claim("FacilityName", facilityName));
                                }

                                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                                var token = new JwtSecurityToken(
                                    _configuration["Jwt:Issuer"],
                                    _configuration["Jwt:Audience"],
                                    claims,
                                    expires: DateTime.UtcNow.AddMinutes(10),
                                    signingCredentials: signIn);

                                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                                var loginResponse = new LoginResponse
                                {
                                    UserMessage = "Login Success",
                                    AccessToken = jwtToken,
                                    Facilities = new List<string>()
                                };
                                loginResponse.Facilities.AddRange(facilityNames);

                                return Ok(loginResponse);
                            }
                            else
                            {
                                return BadRequest("Invalid password");
                            }
                        }
                        else
                        {
                            return BadRequest("User not found");
                        }
                    }
                    else
                    {
                        return BadRequest("Invalid request");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        [HttpGet("getStaffUsers")] //Xem thông tin Nhân viên
        public IActionResult GetStaffUsers()
        {
            try
            {
                var staffRoles = _db.Role.Where(r => r.RoleName == "Staff" || r.RoleName == "Manage").ToList();

                if (staffRoles.Count == 0)
                {
                    return NotFound("Staff roles not found");
                }

                var staffRoleIds = staffRoles.Select(r => r.RoleID).ToList();

                var staffUserIds = _db.UserRole
                    .Where(ur => staffRoleIds.Contains(ur.RoleID))
                    .Select(ur => ur.UserID)
                    .ToList();

                var staffUsers = _db.Users
                    .Where(u => staffUserIds.Contains(u.UserID))
                    .ToList();

                return Ok(staffUsers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("getStaffUsersByFacility")]
        public IActionResult GetStaffUsersByFacility(string facilityName)
        {
            try
            {
                var staffRole = _db.Role.FirstOrDefault(r => r.RoleName == "Staff");

                if (staffRole == null)
                {
                    return NotFound("Staff role not found");
                }

                var staffUserIds = _db.UserRole
                    .Where(ur => ur.RoleID == staffRole.RoleID)
                    .Select(ur => ur.UserID)
                    .ToList();

                var staffUsers = _db.UserFacilities
                    .Include(u => u.User) // Eagerly load the User entity
                    .Include(u => u.Facility) // Eagerly load the Facility entity
                    .Where(u => staffUserIds.Contains(u.UserID) && u.Facility.FacilityName == facilityName) // Filter by facility name
                    .ToList();

                return Ok(staffUsers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("getCustomerUsersByFacility")]
        public IActionResult GetCustomerUsersByFacility(string facilityName)
        {
            try
            {
                var staffRole = _db.Role.FirstOrDefault(r => r.RoleName == "Customer");

                if (staffRole == null)
                {
                    return NotFound("Customer role not found");
                }

                var staffUserIds = _db.UserRole
                    .Where(ur => ur.RoleID == staffRole.RoleID)
                    .Select(ur => ur.UserID)
                    .ToList();

                var staffUsers = _db.UserFacilities
                    .Include(u => u.User) // Eagerly load the User entity
                    .Include(u => u.Facility) // Eagerly load the Facility entity
                    .Where(u => staffUserIds.Contains(u.UserID) && u.Facility.FacilityName == facilityName) // Filter by facility name
                    .ToList();

                return Ok(staffUsers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("getCustomerUsers")] //Xem thông tin khách hàng
        public IActionResult GetCustomerUsers()
        {
            try
            {
                var customerRole = _db.Role.FirstOrDefault(r => r.RoleName == "Customer");

                if (customerRole == null)
                {
                    return NotFound("Customer role not found");
                }

                var customerUserIds = _db.UserRole
                    .Where(ur => ur.RoleID == customerRole.RoleID)
                    .Select(ur => ur.UserID)
                    .ToList();

                var customerUsers = _db.Users
                    .Where(u => customerUserIds.Contains(u.UserID))
                    .ToList();

                return Ok(customerUsers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("assignrole")] // Cho Admin
        public IActionResult AssignRoleToUser([FromBody] AddUserRole userrole)
        {
            try
            {
                var addroles = new List<UserRole>();
                var user = _db.Users.SingleOrDefault(s => s.UserID == userrole.UserId);

                if (user == null)
                {
                    return BadRequest("User is not valid");
                }

                foreach (int role in userrole.RoleIds)
                {
                    var userroleentity = new UserRole
                    {
                        // Không cần chỉ định giá trị cho cột 'UserRoleID'
                        // RoleID và UserID sẽ tự động được gán giá trị
                        RoleID = role,
                        UserID = user.UserID
                    };
                    addroles.Add(userroleentity);
                }

                _db.UserRole.AddRange(addroles);
                _db.SaveChanges();

                return Ok("Roles assigned successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("assignfacilities")] // Cho Admin
        public IActionResult AssignFacilitiesToUser([FromBody] AddUserFacilities obj)
        {
            try
            {
                var user = _db.Users.SingleOrDefault(u => u.UserID == obj.UserId);

                if (user == null)
                {
                    return BadRequest("User is not valid");
                }

                // Xóa bất kỳ bản ghi UserFacilities hiện có cho người dùng này
                var existingUserFacilities = _db.UserFacilities.Where(uf => uf.UserID == obj.UserId);
                _db.UserFacilities.RemoveRange(existingUserFacilities);

                // Tạo các bản ghi UserFacilities mới từ danh sách UserFaci
                var userFacilities = obj.UserFaci.Select(facilityId => new UserFacilities
                {
                    UserID = obj.UserId,
                    FacilityID = facilityId
                }).ToList();

                // Thêm các bản ghi UserFacilities mới vào cơ sở dữ liệu
                _db.UserFacilities.AddRange(userFacilities);
                _db.SaveChanges();

                return Ok("Facilities assigned successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("addStaff")] //Bên Admin đăng ký
        public IActionResult AddUser([FromBody] Users user)
        {
            try
            {
                var existingUser = _db.Users.FirstOrDefault(u => u.UserName == user.UserName || u.Email == user.Email);
                if (existingUser != null)
                {
                    return BadRequest("Username or Email already exists");
                }

                // Hash mật khẩu trước khi thêm người dùng
                user.Password = user.HashPassword(user.Password);

                _db.Users.Add(user);
                _db.SaveChanges();

                // Thêm ID của người dùng vào phần trả về
                var response = new
                {
                    UserID = user.UserID, // Thay đổi tên thuộc tính tùy theo cấu trúc của lớp Users
                    user.UserName,
                    user.Email
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

       

        [HttpPost("register")] // Phương thức đăng ký người dùng
        public IActionResult Register([FromBody] Users user, [FromQuery] string facilityName)
        {
            try
            {
                var existingUser = _db.Users.FirstOrDefault(u => u.UserName == user.UserName || u.Email == user.Email);
                if (existingUser != null)
                {
                    return BadRequest("Username or Email already exists");
                }

                // Hash mật khẩu trước khi thêm người dùng
                user.Password = user.HashPassword(user.Password);

                _db.Users.Add(user);
                _db.SaveChanges();

                // Lấy vai trò "Customer" từ cơ sở dữ liệu
                var customerRole = _db.Role.FirstOrDefault(r => r.RoleName == "Customer");

                if (customerRole != null)
                {
                    // Tạo bản ghi UserRole để gán vai trò "Customer" cho người dùng
                    var userRole = new UserRole
                    {
                        UserID = user.UserID,
                        RoleID = customerRole.RoleID
                    };

                    _db.UserRole.Add(userRole);
                    _db.SaveChanges();
                }

                // Tìm cơ sở đã chọn
                var facility = _db.Facilities.FirstOrDefault(f => f.FacilityName == facilityName);

                if (facility == null)
                {
                    return BadRequest("Invalid facility name");
                }

                // Tạo bản ghi UserFacilities để gán cơ sở cho người dùng
                var userFacilities = new UserFacilities
                {
                    UserID = user.UserID,
                    FacilityID = facility.FacilityID
                };

                _db.UserFacilities.Add(userFacilities);
                _db.SaveChanges();

                // Thêm ID của người dùng vào phần trả về
                var response = new
                {
                    UserID = user.UserID,
                    user.UserName,
                    user.Email,
                    FacilityName = facility.FacilityName
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
