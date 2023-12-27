using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilitiesController : ControllerBase
    {
        private readonly PhotoContext _db;

        public FacilitiesController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Facilities
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Facilities
                .FirstOrDefault(m => m.FacilityID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(Facilities tf)
        {
            try
            {
                _db.Facilities.Add(tf);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult UpdateSchool(int id, Facilities tf)
        {
            if (id != tf.FacilityID)
            {
                return BadRequest("Invalid ServiceID");
            }

            var existingFacilities = _db.Facilities.Find(id);
            if (existingFacilities is null)
            {
                return NotFound("Facilities not found");
            }

            try
            {
                existingFacilities.FacilityName = tf.FacilityName;

                _db.Facilities.Update(existingFacilities);
                _db.SaveChanges();

                return Ok("Facilities updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Facilities: {ex.Message}");
            }
        }

        [HttpDelete("{FacilityId}")]
        public IActionResult Delete(int FacilityId)
        {
            try
            {
                var trainingFacilities = _db.Facilities.Find(FacilityId);
                if (trainingFacilities != null)
                {
                    _db.Facilities.Remove(trainingFacilities);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"TrainingFacilities with ID {FacilityId} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
