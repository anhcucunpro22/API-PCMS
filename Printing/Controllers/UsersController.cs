using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.AppModels;
using Printing.Data;
using Printing.Interfaces;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PhotoContext _db;
        private readonly ICustomerApi _customerApi;
        private readonly IUserRepository _userRepository;
        public UsersController(PhotoContext db, ICustomerApi customerApi, IUserRepository userRepository)
        {

            _db = db;
            _customerApi = customerApi;
            _userRepository = userRepository;
        }

        [HttpGet("SearchbyPhone")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> SearchUserByPhone(string Phone)
        {
            var result = await _customerApi.SearchUserByPhone(Phone);
            return Ok(result);
        }


        [HttpGet("Listname")]
        public async Task<ActionResult> GetCustomers()
        {
            var role = "Customer"; // Tên của vai trò là "Customer"

            var customerNames = await _db.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == role))
                .Select(u => new { FullName = u.FullName })
                .ToListAsync();

            return Ok(customerNames);
        }



        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Users>>> Search(string name)
        {
            try
            {
                var result = await _userRepository.SearchByName(name);
                if (!string.IsNullOrEmpty(result))
                {
                    // Convert the JSON string back to a list of users
                    var users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Users>>(result);
                    return Ok(users);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving data from the database: {ex.Message}");
            }
        }


        [HttpGet] //To do list for Admin
        public JsonResult Get()
        {
            var data = _db.Users
                .Include(m => m.Organization)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Users
                .Include(m => m.Organization)
                .FirstOrDefault(m => m.UserID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(Users use)
        {
            try
            {
                _db.Users.Add(use);
                use.Password = use.HashPassword(use.Password);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult UpdateSchool(int id, Users use)
        {
            if (id != use.UserID)
            {
                return BadRequest("Invalid Users ID");
            }

            var existingUsers = _db.Users.Find(id);
            if (existingUsers is null)
            {
                return NotFound("Users not found");
            }

            try
            {
                existingUsers.FullName = use.FullName;
                existingUsers.UserName = use.UserName;
                existingUsers.Email = use.Email;
                existingUsers.Password = use.HashPassword(use.Password);
                existingUsers.Address = use.Address;
                existingUsers.Phone = use.Phone;
                existingUsers.CodeUser = use.CodeUser;
                existingUsers.Gender = use.Gender;
                existingUsers.Isactive = use.Isactive;
                existingUsers.OrganizationID = use.OrganizationID;

                _db.Users.Update(existingUsers);
                _db.SaveChanges();

                return Ok("Users updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Users: {ex.Message}");
            }
        }

        [HttpPut("users/{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UserUpdateModel updateModel)
        {
            // Retrieve the user from the database
            var user = _db.Users.FirstOrDefault(u => u.UserID == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Update the IsActive property
            user.UpdateIsActive(updateModel);

            // Save the changes to the database
            _db.SaveChanges();

            return Ok(user);
        }

        [HttpPut("userinformation/{userId}")]
        public IActionResult UpdateUserInformation(int userId, [FromBody] UserInforUp userInforUp)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(u => u.UserID == userId);

                if (user == null)
                {
                    return NotFound("Người dùng không tồn tại.");
                }

                user.UserInforUp(userInforUp);

                _db.SaveChanges();

                return Ok("Thông tin người dùng đã được cập nhật.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }



        [HttpDelete("UserID")]
        public IActionResult Delete(int UserID)
        {
            try
            {
                var users = _db.Users.Find(UserID);
                if (users != null)
                {
                    _db.Users.Remove(users);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Users with ID {UserID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
