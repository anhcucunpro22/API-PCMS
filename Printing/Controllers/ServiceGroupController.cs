using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceGroupController : ControllerBase
    {
        private readonly PhotoContext _db;

        public ServiceGroupController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.ServiceGroup
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.ServiceGroup
                .FirstOrDefault(m => m.GroupID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(ServiceGroup serg)
        {
            try
            {
                _db.ServiceGroup.Add(serg);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult UpdateSchool(int id, ServiceGroup serg)
        {
            if (id != serg.GroupID)
            {
                return BadRequest("Invalid GroupID");
            }

            var existingGr = _db.ServiceGroup.Find(id);
            if (existingGr is null)
            {
                return NotFound("School not found");
            }

            try
            {
                existingGr.GroupName = serg.GroupName;

                _db.ServiceGroup.Update(existingGr);
                _db.SaveChanges();

                return Ok("ServiceGroup updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update ServiceGroup: {ex.Message}");
            }
        }

        [HttpDelete("{ServiceGroupID}")]
        public IActionResult Delete(int GroupID)
        {
            try
            {
                var serviceGroups = _db.ServiceGroup.Find(GroupID);
                if (serviceGroups != null)
                {
                    _db.ServiceGroup.Remove(serviceGroups);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"ServiceGroups with ID {GroupID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
