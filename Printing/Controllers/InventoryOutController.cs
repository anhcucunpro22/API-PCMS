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
    public class InventoryOutController : ControllerBase
    {
        private readonly PhotoContext _db;
        public InventoryOutController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.InventoryOut
                .Include(m => m.Facility)
                .Include(n => n.Warehouse)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("ByWarehouseIDAndFacilityID")]
        public IActionResult GetByUserIDAndOrganizationID(int? FacilityID, int? WarehouseID)
        {
            var query = _db.InventoryOut.AsQueryable();

            if (WarehouseID.HasValue)
            {
                query = query.Include(m => m.Warehouse).Where(c => c.WarehouseID == WarehouseID);
            }

            if (FacilityID.HasValue)
            {
                query = query.Include(m => m.Facility).Where(c => c.FacilityID == FacilityID);
            }

            var data = query.ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.InventoryOut
                .Include(m => m.Facility)
                .Include(n => n.Warehouse)
                .FirstOrDefault(m => m.InventoryOutID == id);
            return new JsonResult(data);
        }

        //[HttpPost]
        //public IActionResult Post(InventoryOut ivo, int MaterialID, int UnitID)
        //{
        //    using (var transaction = _db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // ivo.TotalAmount = ivo.AmountReceived + ivo.TaxAmount - ivo.DiscountAmount;

        //            _db.InventoryOut.Add(ivo);
        //            _db.SaveChanges();

        //            // Lấy ID của InventoryOut vừa được thêm vào cơ sở dữ liệu
        //            int newInventoryOutID = ivo.InventoryOutID;

        //            // Tạo một ReceiptDetail và gán InventoryOutID, UnitID và MaterialID của nó bằng ID của InventoryOut, Material và UnitOfMeasure tương ứng
        //            InventoryOutDetail newInventoryOutDetail = new InventoryOutDetail
        //            {
        //                InventoryOutID = newInventoryOutID,
        //                UnitID = UnitID, // Gán ID của UnitOfMeasure tương ứng
        //                MaterialID = MaterialID, // Gán ID của Material tương ứng
        //                                   // Các thuộc tính khác của ReceiptDetail
        //            };

        //            _db.InventoryOutDetail.Add(newInventoryOutDetail);
        //            _db.SaveChanges();

        //            transaction.Commit();

        //            return new JsonResult(new { InventoryOutID = newInventoryOutDetail.InventoryOutID, Message = "Added Successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            return BadRequest($"Error: {ex.Message}");
        //        }
        //    }
        //}

        [HttpPut("update/{id}")]
        public IActionResult UpdateInventoryOut(int id, [FromBody] UpdateOut updateOut)
        {
            try
            {
                var inventoryOut = _db.InventoryOut.Find(id);

                if (inventoryOut == null)
                {
                    return NotFound();
                }

                inventoryOut.WarehouseID = updateOut.WarehouseID;
                inventoryOut.FacilityID = updateOut.FacilityID;
                inventoryOut.PercentageDiscount = updateOut.PercentageDiscount;
                inventoryOut.PercentageTax = updateOut.PercentageTax;
                inventoryOut.PaymentMethod = updateOut.PaymentMethod;

                _db.SaveChanges(); // Lưu thay đổi

                return Ok(inventoryOut); // Trả về thông tin của inventoryOut đã cập nhật
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("{InventoryOutID}")]
        public IActionResult Delete(int InventoryOutID)
        {
            try
            {

                var inventoryOut = _db.InventoryOut.Find(InventoryOutID);
                if (inventoryOut != null)
                {
                    _db.InventoryOut.Remove(inventoryOut);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"InventoryOut with ID {InventoryOutID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
