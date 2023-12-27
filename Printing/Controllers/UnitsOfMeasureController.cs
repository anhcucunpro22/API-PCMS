using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsOfMeasureController : ControllerBase
    {
        private readonly PhotoContext _db;

        public UnitsOfMeasureController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.UnitOfMeasure

                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.UnitOfMeasure

                .FirstOrDefault(m => m.UnitID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(UnitOfMeasure uom)
        {
            try
            {
                _db.UnitOfMeasure.Add(uom);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                // Check the inner exception for more details
                if (ex.InnerException != null)
                {
                    var innerExceptionMessage = ex.InnerException.Message;
                    // Log or print the innerExceptionMessage for further investigation
                }

                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult Update(int id, UnitOfMeasure uom)
        {
            if (id != uom.UnitID)
            {
                return BadRequest("Invalid UnitID");
            }

            var existingUnitOfMeasure = _db.UnitOfMeasure.Find(id);
            if (existingUnitOfMeasure is null)
            {
                return NotFound("UnitOfMeasure not found");
            }

            try
            {
                existingUnitOfMeasure.UnitName = uom.UnitName;
                existingUnitOfMeasure.ConversionFactor = uom.ConversionFactor;
                existingUnitOfMeasure.Description = uom.Description;

                _db.UnitOfMeasure.Update(existingUnitOfMeasure);
                _db.SaveChanges();

                return Ok("UnitOfMeasure updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update UnitOfMeasure: {ex.Message}");
            }
        }

        [HttpDelete("{UnitOfMeasureID}")]
        public IActionResult Delete(int UnitOfMeasureID)
        {
            try
            {

                var unitsOfMeasure = _db.UnitOfMeasure.Find(UnitOfMeasureID);
                if (unitsOfMeasure != null)
                {
                    _db.UnitOfMeasure.Remove(unitsOfMeasure);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"UnitsOfMeasure with ID {UnitOfMeasureID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
