using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printing.Data;
using Printing.Interfaces;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialGroupController : ControllerBase
    {
        private readonly PhotoContext _db;

        public MaterialGroupController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.MaterialGroup
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.MaterialGroup
                .FirstOrDefault(m => m.GroupID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(MaterialGroup mgr)
        {
            try
            {

                _db.MaterialGroup.Add(mgr);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult Update(int id, MaterialGroup mgr)
        {
            if (id != mgr.GroupID)
            {
                return BadRequest("Invalid GroupID");
            }

            var existingMaterialGroup = _db.MaterialGroup.Find(id);
            if (existingMaterialGroup is null)
            {
                return NotFound("MaterialGroup not found");
            }

            try
            {
                existingMaterialGroup.GroupName = mgr.GroupName;

                _db.MaterialGroup.Update(existingMaterialGroup);
                _db.SaveChanges();

                return Ok("MaterialGroup updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update MaterialGroup: {ex.Message}");
            }
        }

        [HttpDelete("{MaterialGroupID}")]
        public IActionResult Delete(int MaterialGroupID)
        {
            try
            {
                var materialGroup = _db.MaterialGroup.Find(MaterialGroupID);
                if (materialGroup != null)
                {
                    _db.MaterialGroup.Remove(materialGroup);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"MaterialGroup with ID {MaterialGroupID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        } 
    }
}
