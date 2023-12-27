using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly PhotoContext _db;

        public SchoolsController(PhotoContext db)
        {

            _db = db;
        }
        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.School
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.School

                .FirstOrDefault(m => m.SchoolID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(School sch)
        {
            try
            {
                _db.School.Add(sch);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
      
        [HttpPut]
        public IActionResult UpdateSchool(int id, School school)
        {
            if (id != school.SchoolID)
            {
                return BadRequest("Invalid school ID");
            }

            var existingSchool = _db.School.Find(id);
            if (existingSchool is null)
            {
                return NotFound("School not found");
            }

            try
            {
                existingSchool.SchoolName = school.SchoolName;

                _db.School.Update(existingSchool);
                _db.SaveChanges();

                return Ok("School updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update school: {ex.Message}");
            }
        }

        [HttpDelete("{SchoolID}")]
        public IActionResult Delete(int SchoolID)
        {
            try
            {
                var schools = _db.School.Find(SchoolID);
                if (schools != null)
                {
                    _db.School.Remove(schools);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"School with ID {SchoolID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }

    }
}
