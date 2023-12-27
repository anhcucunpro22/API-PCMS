using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly PhotoContext _db;

        public MaterialController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Material
                .Include(m => m.Group)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("GetbyMaterialGr")]
        public JsonResult GetbyMaterialGr(int MaterialGroupID)
        {
            var data = _db.Material
                .Include(m => m.Group)
                .Where(o => o.GroupID == MaterialGroupID)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Material
                .Include(m => m.Group)
                .FirstOrDefault(m => m.MaterialID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(Material mat)
        {
            try
            {

                _db.Material.Add(mat);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }

        [HttpPut]
        public IActionResult Update(int id, Material mat)
        {
            if (id != mat.MaterialID)
            {
                return BadRequest("Invalid MaterialID");
            }

            var existingMaterial = _db.Material.Find(id);
            if (existingMaterial is null)
            {
                return NotFound("Material not found");
            }

            try
            {
                existingMaterial.MaterialName = mat.MaterialName;
                existingMaterial.Price = mat.Price;

                _db.Material.Update(existingMaterial);
                _db.SaveChanges();

                return Ok("Material updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Material: {ex.Message}");
            }
        }

        [HttpDelete("{MaterialID}")]
        public IActionResult Delete(int MaterialID)
        {
            try
            {

                var material = _db.Material.Find(MaterialID);
                if (material != null)
                {
                    _db.Material.Remove(material);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Material with ID {MaterialID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
