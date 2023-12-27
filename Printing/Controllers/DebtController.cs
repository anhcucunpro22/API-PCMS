using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.Data;
using Printing.Models;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtController : ControllerBase
    {
        private readonly PhotoContext _db;
        public DebtController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Debt
                .Include(m => m.User)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")] //Get id của công nợ
        public JsonResult Get(int id)
        {
            var data = _db.Debt
                .Include(m => m.User)
                .FirstOrDefault(m => m.DebtID == id);
            return new JsonResult(data);
        }

        [HttpGet("Getbycustomer")] //có thể get id của người dùng
        public JsonResult GetbyUser(int UserID)
        {
            var data = _db.Debt
                .Include(m => m.User)
                .Where(o => o.UserID == UserID)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("getdate")]
        public IActionResult GetDebtByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var debt = _db.Debt
                    .Where(rd => rd.InvoiceDate >= startDate && rd.InvoiceDate <= endDate)
                    .Select(rd => new
                    {
                        rd.DebtID,
                        rd.UserID,
                        rd.InvoiceDate,
                        rd.DebtAmount,
                        rd.PaymentMethod,
                        rd.Status,
                        rd.DueDate,
                        rd.User
                    })
                    .ToList();

                return Ok(debt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Post(Debt dec)
        {
            try
            {

                _db.Debt.Add(dec);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpPut("debts/{debtId}")]
        public IActionResult UpdateDebt(int debtId, [FromBody] Debt updatedDebt)
        {
            try
            {
                var existingDebt = _db.Debt.FirstOrDefault(d => d.DebtID == debtId);

                if (existingDebt == null)
                {
                    return NotFound("Không tìm thấy khoản nợ.");
                }

                // Cập nhật các thuộc tính của khoản nợ
                existingDebt.PaymentMethod = updatedDebt.PaymentMethod;
                existingDebt.Status = updatedDebt.Status;
                existingDebt.DueDate = DateTime.Now; // Cập nhật DueDate thành ngày hiện tại

                // Kiểm tra nếu Status là "Paid", thiết lập DebtAmount về 0
                if (existingDebt.Status == "Paid")
                {
                    existingDebt.DebtAmount = 0;
                }

                _db.SaveChanges();

                return Ok("Cập nhật khoản nợ thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpDelete("{DebtID}")]
        public IActionResult Delete(int DebtID)
        {
            try
            {
                var debtCollection = _db.Debt.Find(DebtID);
                if (debtCollection != null)
                {
                    _db.Debt.Remove(debtCollection);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Debt with ID {DebtID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
