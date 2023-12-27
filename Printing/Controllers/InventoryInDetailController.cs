using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryInDetailController : ControllerBase
    {
        private readonly PhotoContext _db;
        public InventoryInDetailController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.InventoryInDetail
                .Include(m => m.InventoryIn)
                .Include(n => n.Material)
                 .Include(n => n.Unit)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("ByInventoryInIDAndMaterialID")]
        public IActionResult GetByUserIDAndOrganizationID(int? InventoryInID, int? MaterialID, int? UnitOfMeasureID)
        {
            var query = _db.InventoryInDetail.AsQueryable();

            if (InventoryInID.HasValue)
            {
                query = query.Include(m => m.InventoryIn).Where(c => c.InventoryInID == InventoryInID);
            }

            if (MaterialID.HasValue)
            {
                query = query.Include(m => m.Material).Where(c => c.MaterialID == MaterialID);
            }

            if (UnitOfMeasureID.HasValue)
            {
                query = query.Include(m => m.Unit).Where(c => c.UnitID == UnitOfMeasureID);
            }

            var data = query.ToList();
            return new JsonResult(data);
        }

        [HttpGet("getdate")]
        public IActionResult GetInventoryInDetailByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var inventoryInDetail = _db.InventoryInDetail
                    .Where(rd => rd.CreatedDate >= startDate && rd.CreatedDate <= endDate)
                    .Select(rd => new
                    {
                        rd.InventoryInDeID,
                        rd.InventoryInID,
                        rd.MaterialID,
                        rd.UnitID,
                        rd.Quantity,
                        rd.FinalPrice,
                        rd.CreatedBy,
                        rd.CreatedDate,
                        rd.ModifiedDate,
                        rd.Description,
                        rd.InventoryIn,
                        rd.Material,
                        rd.Unit
                    })
                    .ToList();

                return Ok(inventoryInDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.InventoryInDetail
                .Include(m => m.InventoryIn)
                .Include(n => n.Material)
                 .Include(n => n.Unit)
                .FirstOrDefault(m => m.InventoryInDeID == id);
            return new JsonResult(data);
        }

        [HttpPost("detail")]
        public async Task<IActionResult> CreateInventoryInDetail([FromBody] List<InventoryInDetail> inventoryInDetails)
        {
            try
            {
                var inventoryIn = new InventoryIn
                {
                    InDate = DateTime.Now
                    // Thêm các thông tin khác cho InventoryIn nếu cần
                };

                Random random = new Random();
                inventoryIn.InNumber = random.Next(100000, 999999);

                _db.InventoryIn.Add(inventoryIn);
                await _db.SaveChangesAsync();

                var responseData = new List<object>();

                foreach (var inventoryInDetail in inventoryInDetails)
                {
                    var material = await _db.Material.FindAsync(inventoryInDetail.MaterialID);

                    if (material == null)
                    {
                        return NotFound($"Vật liệu với ID {inventoryInDetail.MaterialID} không tồn tại.");
                    }

                    inventoryInDetail.FinalPrice = inventoryInDetail.Quantity * material.Price;
                    inventoryInDetail.InventoryInID = inventoryIn.InventoryInID; // Gán cùng InventoryInID cho InventoryInDetail

                    _db.InventoryInDetail.Add(inventoryInDetail);
                    await _db.SaveChangesAsync();

                    responseData.Add(new
                    {
                        InventoryInDeID = inventoryInDetail.InventoryInDeID,
                        InventoryInID = inventoryInDetail.InventoryInID
                    });
                }

                // Tính tổng FinalPrice của các InventoryInDetail và cập nhật AmountReceived cho InventoryIn
                inventoryIn.AmountReceived = inventoryInDetails.Sum(rd => rd.FinalPrice ?? 0);
                await _db.SaveChangesAsync();

                return Ok(new { data = responseData, message = "Tạo InventoryInDetail thành công.", InventoryInID = inventoryIn.InventoryInID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        [HttpDelete("{InventoryInDetailID}")]
        public IActionResult Delete(int InventoryInDeID)
        {
            try
            {

                var inventoryInDetails = _db.InventoryInDetail.Find(InventoryInDeID);
                if (inventoryInDetails != null)
                {
                    _db.InventoryInDetail.Remove(inventoryInDetails);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"InventoryInDetails with ID {InventoryInDeID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
