using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.AppModels;
using Printing.Data;
using Printing.Models;
using System;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptDetailController : ControllerBase
    {
        private readonly PhotoContext _db;
        public ReceiptDetailController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.ReceiptDetail
                .Include(m => m.Photocopier)
                .Include(m => m.Receipt)
                .Include(m => m.Services)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("getdate")]
        public IActionResult GetReceiptDetailsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var receiptDetails = _db.ReceiptDetail
                    .Where(rd => rd.CreatedDate >= startDate && rd.CreatedDate <= endDate)
                    .Select(rd => new
                    {
                        rd.ReceiptDeID,
                        rd.Quantity,
                        rd.QuantitySets,
                        rd.FinalPrice,
                        rd.CreatedBy,
                        rd.CreatedDate,
                        rd.ModifiedDate,
                        rd.Description,
                        rd.ServiceID,
                        rd.ReceiptID,
                        rd.PhotocopierID,
                        rd.Photocopier,
                        rd.Receipt,
                        rd.Services,
                    })
                    .ToList();

                return Ok(receiptDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpGet("ByReceiptIDAndServiceID")]
        public IActionResult GetByUserIDAndOrganizationID(int? ReceiptID, int? ServiceID, int? PhotocopierID)
        {
            var query = _db.ReceiptDetail.AsQueryable();

            if (ReceiptID.HasValue)
            {
                query = query.Include(m => m.Receipt).Where(c => c.ReceiptID == ReceiptID);
            }

            if (ServiceID.HasValue)
            {
                query = query.Include(m => m.Services).Where(c => c.ServiceID == ServiceID);
            }

            if (PhotocopierID.HasValue)
            {
                query = query.Include(m => m.Photocopier).Where(c => c.PhotocopierID == PhotocopierID);
            }

            var data = query.ToList();
            return new JsonResult(data);
        }


        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.ReceiptDetail
                .Include(m => m.Photocopier)
                .Include(m => m.Receipt)
                .Include(m => m.Services)
                .FirstOrDefault(m => m.ReceiptDeID == id);
            if (data == null)
            {
                return new JsonResult("ReceiptDetail not found");
            }
            return new JsonResult(data);
        }

        [HttpGet("users/{fullName}/receipts")]
        public IActionResult GetReceipts(string fullName)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(u => u.FullName == fullName);

                if (user == null)
                {
                    return NotFound("Người dùng không tồn tại.");
                }

                var receiptDetails = _db.ReceiptDetail
                    .Join(_db.Receipt,
                        rd => rd.ReceiptID,
                        r => r.ReceiptID,
                        (rd, r) => new { ReceiptDetail = rd, r.ReceiptNumber })
                    .Where(joinData => joinData.ReceiptDetail.CreatedBy == fullName)
                    .ToList();

                var receiptCount = receiptDetails.Count;

                var receiptList = receiptDetails.Select(joinData => new
                {
                    ReceiptNumber = joinData.ReceiptNumber,
                    ReceiptDetail = joinData.ReceiptDetail
                }).ToList();

                return Ok(new
                {
                    FullName = fullName,
                    ReceiptCount = receiptCount,
                    ReceiptList = receiptList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        [HttpPost("Receipt")]
        public async Task<IActionResult> CreateForReceiptDetail(ReceiptDetail receiptDetail)
        {
            try
            {
                // Lấy dịch vụ từ cơ sở dữ liệu
                var service = await _db.Services.FindAsync(receiptDetail.ServiceID);

                if (service == null)
                {
                    return NotFound("Dịch vụ không tồn tại.");
                }

                // Tính toán finalprice
                receiptDetail.FinalPrice = (receiptDetail.Quantity * service.Price) * receiptDetail.QuantitySets;

                // Kiểm tra xem receiptid đã được khai báo chưa
                if (receiptDetail.ReceiptID == 0)
                {
                    // Phương pháp 1: Tạo một receipt mới và gán giá trị mặc định cho receiptid
                    var newReceipt = new Receipt
                    {
                        ReceiptDate = DateTime.Now,
                    };

                    Random random = new Random();
                    newReceipt.ReceiptNumber = random.Next(100000, 999999);
                    newReceipt.AmountReceived = receiptDetail.FinalPrice;

                    _db.Receipt.Add(newReceipt);
                    await _db.SaveChangesAsync();

                    receiptDetail.ReceiptID = newReceipt.ReceiptID;
                }

                // Lưu thông tin receiptDetail vào cơ sở dữ liệu
                _db.ReceiptDetail.Add(receiptDetail);
                await _db.SaveChangesAsync();

                var receipt = await _db.Receipt.FindAsync(receiptDetail.ReceiptID);
                if (receipt != null)
                {
                    decimal totalAmount = receipt.ReceiptDetails.Sum(rd => rd.FinalPrice ?? 0);

                    receipt.AmountReceived = totalAmount;
                    await _db.SaveChangesAsync();
                }

                var responseData = new
                {
                    receiptDetailID = receiptDetail.ReceiptDeID,
                    receiptID = receiptDetail.ReceiptID
                };

                return Ok(new { data = responseData, message = "Tạo receiptDetail thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        [HttpPost("GetServices")]
        public async Task<List<Services>> GetServices(List<int> serviceIds)
        {
            // Truy vấn dữ liệu từ cơ sở dữ liệu theo serviceIds
            var services = await _db.Services.Where(s => serviceIds.Contains(s.ServiceID)).ToListAsync();

            return services;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceiptDetails(List<ReceiptDetail> receiptDetails)
        {
            try
            {
                var serviceIds = receiptDetails.Select(rd => rd.ServiceID).Distinct().ToList();
                var services = await GetServices(serviceIds);

                var requestBodies = receiptDetails.Select(rd => new
                {
                    ServiceID = rd.ServiceID,
                    Quantity = rd.Quantity,
                    QuantitySets = rd.QuantitySets
                }).ToList();

                foreach (var receiptDetail in receiptDetails)
                {
                    var service = services.FirstOrDefault(s => s.ServiceID == receiptDetail.ServiceID);

                    if (service == null)
                    {
                        return NotFound("Dịch vụ không tồn tại.");
                    }

                    // Calculate final price
                    receiptDetail.FinalPrice = ((receiptDetail.Quantity * service.Price) * receiptDetail.QuantitySets);

                    // Các bước lưu thông tin ReceiptDetail vào cơ sở dữ liệu...
                }

                return Ok(requestBodies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost("detail")]
        public async Task<IActionResult> CreateMultipleReceiptDetails([FromBody] List<ReceiptDetail> receiptDetails)
        {
            try
            {
                var receipt = new Receipt
                {
                    ReceiptDate = DateTime.Now
                    // Thêm các thông tin khác cho Receipt nếu cần
                };

                Random random = new Random();
                receipt.ReceiptNumber = random.Next(100000, 999999);

                _db.Receipt.Add(receipt);
                await _db.SaveChangesAsync();

                var responseData = new List<object>();

                foreach (var receiptDetail in receiptDetails)
                {
                    var service = await _db.Services.FindAsync(receiptDetail.ServiceID);

                    if (service == null)
                    {
                        return NotFound($"Dịch vụ với ID {receiptDetail.ServiceID} không tồn tại.");
                    }

                    receiptDetail.FinalPrice = (receiptDetail.Quantity * service.Price) * receiptDetail.QuantitySets;

                    receiptDetail.ReceiptID = receipt.ReceiptID; // Gán cùng ReceiptID cho các ReceiptDetail

                    _db.ReceiptDetail.Add(receiptDetail);
                    await _db.SaveChangesAsync();

                    responseData.Add(new
                    {
                        receiptDetailID = receiptDetail.ReceiptDeID,
                        receiptID = receiptDetail.ReceiptID
                    });
                }

                receipt.AmountReceived = receiptDetails.Sum(rd => rd.FinalPrice ?? 0); // Tính tổng FinalPrice của các ReceiptDetail
                await _db.SaveChangesAsync();

                return Ok(new { data = responseData, message = "Tạo receiptDetail thành công.", receiptID = receipt.ReceiptID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }




        [HttpDelete("{ReceiptDetailID}")]
        public IActionResult Delete(int ReceiptID)
        {
            try
            {
                var receiptDetail = _db.ReceiptDetail.Find(ReceiptID);
                if (receiptDetail != null)
                {
                    _db.ReceiptDetail.Remove(receiptDetail);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"ReceiptDetail with ID {ReceiptID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
