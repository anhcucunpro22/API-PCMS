using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly PhotoContext _db;

        public OrganizationsController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Organizations
                .Include(m => m.School)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("Getbyshool")]
        public JsonResult Getbyshool(int schoolID)
        {
            var data = _db.Organizations
                .Include(m => m.School)
                .Where(o => o.SchoolID == schoolID)
                .ToList();
            return new JsonResult(data);
        }


        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Organizations
                .Include(m => m.School)
                .FirstOrDefault(m => m.OrganizationID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(Organizations org)
        {
            try
            {
                _db.Organizations.Add(org);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult UpdateSchool(int id, Organizations org)
        {
            if (id != org.OrganizationID)
            {
                return BadRequest("Invalid Organizations ID");
            }

            var existingOrganizationsl = _db.Organizations.Find(id);
            if (existingOrganizationsl is null)
            {
                return NotFound("Organizations not found");
            }

            try
            {
                existingOrganizationsl.OrganizationName = org.OrganizationName;
                existingOrganizationsl.Location = org.Location;
                existingOrganizationsl.ContactPerson = org.ContactPerson;
                existingOrganizationsl.PhoneNumber = org.PhoneNumber;
                existingOrganizationsl.SchoolID = org.SchoolID;

                _db.Organizations.Update(existingOrganizationsl);
                _db.SaveChanges();

                return Ok("Organizations updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Organizations: {ex.Message}");
            }
        }

        [HttpDelete("{OrganizationID}")]
        public IActionResult Delete(int OrganizationID)
        {
            try
            {
                var organizations = _db.Organizations.Find(OrganizationID);
                if (organizations != null)
                {
                    _db.Organizations.Remove(organizations);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Organizations with ID {OrganizationID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
