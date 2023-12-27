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
    public class InventoryOutDetailsController : ControllerBase
    {
        private readonly PhotoContext _db;
        public InventoryOutDetailsController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.InventoryOutDetail
                .Include(n => n.InventoryOut)
                 .Include(n => n.Material)
                 .Include(n => n.Unit)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("ByInventoryOutIDAndMaterialID")]
        public IActionResult GetByUserIDAndOrganizationID(int? InventoryOutID, int? MaterialID, int? UnitOfMeasureID)
        {
            var query = _db.InventoryOutDetail.AsQueryable();

            if (InventoryOutID.HasValue)
            {
                query = query.Include(m => m.InventoryOut).Where(c => c.InventoryOutID == InventoryOutID);
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
        public IActionResult GetInventoryOutDetailByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var inventoryOutDetail = _db.InventoryOutDetail
                    .Where(rd => rd.CreatedDate >= startDate && rd.CreatedDate <= endDate)
                    .Select(rd => new
                    {
                        rd.InventoryOutDeID,
                        rd.InventoryOutID,
                        rd.MaterialID,
                        rd.UnitID,
                        rd.Quantity,
                        rd.TotalAmount,
                        rd.CreatedBy,
                        rd.CreatedDate,
                        rd.ModifiedDate,
                        rd.Description,
                        rd.InventoryOut,
                        rd.Material,
                        rd.Unit
                    })
                    .ToList();

                return Ok(inventoryOutDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.InventoryOutDetail
                .Include(n => n.InventoryOut)
                 .Include(n => n.Material)
                 .Include(n => n.Unit)
                .FirstOrDefault(m => m.InventoryOutDeID == id);
            return new JsonResult(data);
        }

        [HttpGet("remainingquantity")]
        public async Task<IActionResult> GetAllRemainingQuantities()
        {
            try
            {
                // Lấy danh sách tất cả các tên vật liệu
                var materialNames = await _db.Material.Select(m => m.MaterialName).ToListAsync();

                var remainingQuantities = new Dictionary<string, int>();

                // Duyệt qua từng tên vật liệu và tính số lượng còn lại
                foreach (var materialName in materialNames)
                {
                    var material = await _db.Material.FirstOrDefaultAsync(m => m.MaterialName == materialName);

                    if (material != null)
                    {
                        int materialId = material.MaterialID;

                        int totalQuantityIn = await _db.InventoryInDetail
                            .Where(inventory => inventory.MaterialID == materialId)
                            .SumAsync(inventory => inventory.Quantity ?? 0);

                        int totalQuantityOut = await _db.InventoryOutDetail
                            .Where(inventory => inventory.MaterialID == materialId)
                            .SumAsync(inventory => inventory.Quantity ?? 0);

                        int remainingQuantity = totalQuantityIn - totalQuantityOut;

                        remainingQuantities.Add(materialName, remainingQuantity);
                    }
                }

                return Ok(remainingQuantities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }



        [HttpPost("Quantity")]
        public async Task<IActionResult> UpdateInventory([FromBody] InventoryUpdateRequest request)
        {
            if (request != null)
            {
                int warehouseId = request.WarehouseId;

                // Lấy danh sách chi tiết nhập kho cho WarehouseId
                var inventoryInDetails = await _db.InventoryInDetail
                    .Include(inventory => inventory.Material)
                    .Where(inventory => inventory.InventoryIn.WarehouseID == warehouseId)
                    .ToListAsync();

                // Tạo một từ điển để lưu trữ số lượng theo MaterialName
                var quantityByMaterialName = new Dictionary<string, int>();

                // Tính tổng số lượng từng MaterialName
                foreach (var inventoryInDetail in inventoryInDetails)
                {
                    string materialName = inventoryInDetail.Material.MaterialName;

                    if (quantityByMaterialName.ContainsKey(materialName))
                    {
                        quantityByMaterialName[materialName] += inventoryInDetail.Quantity ?? 0;
                    }
                    else
                    {
                        quantityByMaterialName[materialName] = inventoryInDetail.Quantity ?? 0;
                    }
                }

                // Trả về kết quả thành công với số lượng từng MaterialName
                return Ok(quantityByMaterialName);
            }

            // Trả về BadRequest nếu request không hợp lệ
            return BadRequest();
        }


        [HttpPost("detail")]
        public async Task<IActionResult> CreateInventoryOutDetail([FromBody] List<InventoryOutDetail> inventoryOutDetails)
        {
            try
            {
                var inventoryOut = new InventoryOut
                {
                    OutDate = DateTime.Now
                    // Thêm các thông tin khác cho inventoryOut nếu cần
                };

                Random random = new Random();
                inventoryOut.OutNumber = random.Next(100000, 999999);

                _db.InventoryOut.Add(inventoryOut);
                await _db.SaveChangesAsync();

                var responseData = new List<object>();

                foreach (var inventoryOutDetail in inventoryOutDetails)
                {
                    var material = await _db.Material.FindAsync(inventoryOutDetail.MaterialID);

                    if (material == null)
                    {
                        return NotFound($"Vật liệu với ID {inventoryOutDetail.MaterialID} không tồn tại.");
                    }

                    inventoryOutDetail.TotalAmount = inventoryOutDetail.Quantity * material.Price;
                    inventoryOutDetail.InventoryOutID = inventoryOut.InventoryOutID; // Gán cùng InventoryOutID cho InventoryOutDetail

                    _db.InventoryOutDetail.Add(inventoryOutDetail);
                    await _db.SaveChangesAsync();

                    responseData.Add(new
                    {
                        InventoryOutDeID = inventoryOutDetail.InventoryOutDeID,
                        InventoryOutID = inventoryOutDetail.InventoryOutID
                    });
                }

                // Tính tổng TotalAmount của các InventoryOutDetail và cập nhật AmountReceived cho InventoryOut
                inventoryOut.AmountReceived = inventoryOutDetails.Sum(rd => rd.TotalAmount ?? 0);
                await _db.SaveChangesAsync();

                return Ok(new { data = responseData, message = "Tạo InventoryOutDetail thành công.", InventoryOutID = inventoryOut.InventoryOutID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }



        [HttpDelete("{InventoryOutDeID}")]
        public IActionResult Delete(int InventoryOutDeID)
        {
            try
            {

                var inventoryOutDetails = _db.InventoryOutDetail.Find(InventoryOutDeID);
                if (inventoryOutDetails != null)
                {
                    _db.InventoryOutDetail.Remove(inventoryOutDetails);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"InventoryOutDetails with ID {InventoryOutDeID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }

    }
}
