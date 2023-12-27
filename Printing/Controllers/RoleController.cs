using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly PhotoContext _db;

        public RoleController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Role
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Role
                .FirstOrDefault(m => m.RoleID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(Role ro)
        {
            try
            {
                _db.Role.Add(ro);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult Update(int id, Role ro)
        {
            if (id != ro.RoleID)
            {
                return BadRequest("Invalid RoleID");
            }

            var existingRole = _db.Role.Find(id);
            if (existingRole is null)
            {
                return NotFound("Role not found");
            }

            try
            {
                existingRole.RoleName = ro.RoleName;

                _db.Role.Update(existingRole);
                _db.SaveChanges();

                return Ok("Role updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Role: {ex.Message}");
            }
        }

        [HttpDelete("{RoleID}")]
        public IActionResult Delete(int RoleID)
        {
            try
            {
                var role = _db.Role.Find(RoleID);
                if (role != null)
                {
                    _db.Role.Remove(role);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Service with ID {RoleID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
