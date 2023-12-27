using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly PhotoContext _db;

        public SuppliersController(PhotoContext db)
        {

            _db = db;
        }


        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Suppliers
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Suppliers
                .FirstOrDefault(m => m.SupplierID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(Suppliers sl)
        {
            try
            {
                _db.Suppliers.Add(sl);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult Update(int id, Suppliers sl)
        {
            if (id != sl.SupplierID)
            {
                return BadRequest("Invalid SupplierID");
            }

            var existingSuppliers = _db.Suppliers.Find(id);
            if (existingSuppliers is null)
            {
                return NotFound("Suppliers not found");
            }

            try
            {
                existingSuppliers.SupplierName = sl.SupplierName;
                existingSuppliers.Address = sl.Address;
                existingSuppliers.ContactName = sl.ContactName;
                existingSuppliers.Phone = sl.Phone;

                _db.Suppliers.Update(existingSuppliers);
                _db.SaveChanges();

                return Ok("Suppliers updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Suppliers: {ex.Message}");
            }
        }

        [HttpDelete("{SupplierID}")]
        public IActionResult Delete(int SupplierID)
        {
            try
            {
                var suppliers = _db.Suppliers.Find(SupplierID);
                if (suppliers != null)
                {
                    _db.Suppliers.Remove(suppliers);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Suppliers with ID {SupplierID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }


    }
}
