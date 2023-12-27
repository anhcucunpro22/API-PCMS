using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.AppModels;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryInController : ControllerBase
    {
        private readonly PhotoContext _db;
        public InventoryInController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.InventoryIn
                .Include(m => m.Warehouse)
                .Include(n => n.Supplier)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("ByWarehouseIDAndSupplierID")]
        public IActionResult GetByUserIDAndOrganizationID(int? WarehouseID, int? SupplierID)
        {
            var query = _db.InventoryIn.AsQueryable();

            if (WarehouseID.HasValue)
            {
                query = query.Include(m => m.Warehouse).Where(c => c.WarehouseID == WarehouseID);
            }

            if (SupplierID.HasValue)
            {
                query = query.Include(m => m.Supplier).Where(c => c.SupplierID == SupplierID);
            }

            var data = query.ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.InventoryIn
                .Include(m => m.Warehouse)
                .Include(n => n.Supplier)
                .FirstOrDefault(m => m.InventoryInID == id);
            return new JsonResult(data);
        }

        //[HttpPost]
        //public IActionResult Post(InventoryIn ivi, int MaterialID, int UnitID)
        //{
        //    using (var transaction = _db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // ivi.TotalAmount = ivi.AmountReceived + ivi.TaxAmount - ivi.DiscountAmount;

        //            _db.InventoryIn.Add(ivi);
        //            _db.SaveChanges();

        //            // Lấy ID của InventoryInID vừa được thêm vào cơ sở dữ liệu
        //            int newInventoryInID = ivi.InventoryInID;

        //            // Tạo một ReceiptDetail và gán InventoryInID, UnitID và MaterialID của nó bằng ID của InventoryIn, Material và UnitOfMeasure tương ứng
        //            InventoryInDetail newInventoryInDetail = new InventoryInDetail
        //            {
        //                InventoryInID = newInventoryInID,
        //                UnitID = UnitID, // Gán ID của UnitOfMeasure tương ứng
        //                MaterialID = MaterialID, // Gán ID của Material tương ứng
        //                                         // Các thuộc tính khác của InventoryInDetail
        //            };

        //            _db.InventoryInDetail.Add(newInventoryInDetail);
        //            _db.SaveChanges();

        //            transaction.Commit();

        //            return new JsonResult(new { InventoryInID = newInventoryInDetail.InventoryInID, Message = "Added Successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            return BadRequest($"Error: {ex.Message}");
        //        }
        //    }
        //}

        [HttpPost]
        public IActionResult Post(InventoryIn InventoryIn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.InventoryIn.Add(InventoryIn);
                    _db.SaveChanges();

                    return new JsonResult(new { InventoryIn = InventoryIn.InventoryInID, Message = "Added Successfully" });
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error: {ex.Message}");
                }
            }
            else
            {
                return BadRequest("Invalid data");
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateInventoryIn(int id, [FromBody] UpdateIn updateIn)
        {
            try
            {
                var inventoryIn = _db.InventoryIn.Find(id);

                if (inventoryIn == null)
                {
                    return NotFound();
                }

                inventoryIn.WarehouseID = updateIn.WarehouseID;
                inventoryIn.SupplierID = updateIn.SupplierID;
                inventoryIn.PercentageDiscount = updateIn.PercentageDiscount;
                inventoryIn.PercentageTax = updateIn.PercentageTax;
                inventoryIn.PaymentMethod = updateIn.PaymentMethod;

                _db.SaveChanges(); // Lưu thay đổi

                return Ok(inventoryIn); // Trả về thông tin của InventoryIn đã cập nhật
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpDelete("{InventoryInID}")]
        public IActionResult Delete(int InventoryInID)
        {
            try
            {

                var inventoryIn = _db.InventoryIn.Find(InventoryInID);
                if (inventoryIn != null)
                {
                    _db.InventoryIn.Remove(inventoryIn);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"InventoryIn with ID {InventoryInID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
