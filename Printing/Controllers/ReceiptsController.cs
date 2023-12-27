using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.AppModels;
using Printing.Data;
using Printing.Models;
using System.Collections;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly PhotoContext _db;
        public ReceiptsController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Receipt
                .Include(m => m.User)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("ByCustomer")]
        public IActionResult ByReceipts(int? UserID)
        {
            var query = _db.Receipt.AsQueryable();

            if (UserID.HasValue)
            {
                query = query.Include(m => m.User).Where(c => c.UserID == UserID);
            }

            var data = query.ToList();
            return new JsonResult(data);
        }

        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Receipt
            .Include(m => m.User)
            .FirstOrDefault(m => m.UserID == id);
            return new JsonResult(data);
        }

        [HttpGet("getdate")]
        public IActionResult GetReceiptByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var receipt = _db.Receipt
                    .Where(rd => rd.ReceiptDate >= startDate && rd.ReceiptDate <= endDate)
                    .Select(rd => new
                    {
                        rd.ReceiptID,
                        rd.UserID,
                        rd.ReceiptDate,
                        rd.ReceiptNumber,
                        rd.AmountReceived,
                        rd.PercentageDiscount,
                        rd.DiscountAmount,
                        rd.DepositPayment,
                        rd.PercentageTax,
                        rd.TaxAmount,
                        rd.TotalAmount,
                        rd.PaymentMethod,
                        rd.User
                    })
                    .ToList();

                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpGet("receipts/{idRe}")] // Modify the route template to be unique
        public ActionResult<Receipt> GetReceiptByID(int idRe)
        {
            Receipt receipt = _db.Receipt
                .Include(r => r.User)
                .FirstOrDefault(r => r.ReceiptID == idRe);

            if (receipt == null)
            {
                return NotFound(); // Handle the case when the receipt is not found
            }

            return Ok(receipt); // Return the entire receipt object as the response
        }

        //[HttpPost]
        //public IActionResult Post(Receipt rec, int photocopierID, int serviceID)
        //{
        //    using (var transaction = _db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Generate a random ReceiptNumber
        //            Random random = new Random();
        //            rec.ReceiptNumber = random.Next(100000, 999999);

        //            rec.TotalAmount = rec.AmountReceived + rec.TaxAmount - rec.DiscountAmount - rec.DepositPayment;

        //            _db.Receipt.Add(rec);
        //            _db.SaveChanges();

        //            // Retrieve the newly added receipt's ID
        //            int newReceiptID = rec.ReceiptID;

        //            // Create a new ReceiptDetail and assign its ReceiptID, PhotocopierID, and ServiceID with the corresponding IDs
        //            ReceiptDetail newReceiptDetail = new ReceiptDetail
        //            {
        //                ReceiptID = newReceiptID,
        //                PhotocopierID = photocopierID,
        //                ServiceID = serviceID
        //                // Other properties of ReceiptDetail
        //            };

        //            _db.ReceiptDetail.Add(newReceiptDetail);
        //            _db.SaveChanges();

        //            if (rec.TotalAmount != 0)
        //            {
        //                // Create a new Debt entry
        //                Debt newDebt = new Debt
        //                {
        //                    UserID = rec.UserID, // Assign the UserID from the Receipt
        //                    DebtAmount = rec.TotalAmount, // Assign the TotalAmount from the Receipt
        //                    InvoiceDate = rec.CreatedDate, // Assign the InvoiceDate from the Receipt
        //                    CollectionDate = rec.ModifiedDate,
        //                    Status = "Pending" // Set the initial status as "Pending" or any other desired value
        //                                       // Other properties of Debt
        //                };

        //                _db.Debt.Add(newDebt);
        //                _db.SaveChanges();
        //            }

        //            transaction.Commit();

        //            return new JsonResult(new { ReceiptID = newReceiptDetail.ReceiptID, ReceiptDeID = newReceiptDetail.ReceiptDeID, ReceiptNumber = rec.ReceiptNumber, Message = "Added Successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            return BadRequest($"Error: {ex.Message}");
        //        }
        //    }
        //}



        [HttpPut]
        public IActionResult Update(int id, Receipt rec)
        {
            if (id != rec.ReceiptID)
            {
                return BadRequest("Invalid ReceiptID");
            }

            var existingReceipt = _db.Receipt.Find(id);
            if (existingReceipt is null)
            {
                return NotFound("Receipt not found");
            }

            try
            {
                existingReceipt.ReceiptDate = rec.ReceiptDate;
                existingReceipt.UserID = rec.UserID;

                _db.Receipt.Update(existingReceipt);
                _db.SaveChanges();

                return Ok("Receipt updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Receipt: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateAddUser updateUser)
        {
            try
            {
                var receipt = _db.Receipt.Find(id);

                if (receipt == null)
                {
                    return NotFound();
                }

                receipt.UpdateAddUser(updateUser);

                _db.SaveChanges();

                if (receipt.TotalAmount != 0)
                {
                    // Create a new Debt entry
                    Debt newDebt = new Debt
                    {
                        UserID = receipt.UserID, // Assign the UserID from the Receipt
                        DebtAmount = receipt.TotalAmount, // Assign the TotalAmount from the Receipt
                        InvoiceDate = receipt.ReceiptDate, // Assign the InvoiceDate from the Receipt
                        Status = "Pending" // Set the initial status as "Pending" or any other desired value
                                           // Other properties of Debt
                    };

                    _db.Debt.Add(newDebt);
                    _db.SaveChanges();
                }

                return Ok(receipt);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("{ReceiptID}")]
        public IActionResult Delete(int ReceiptID)
        {
            try
            {
                var receipts = _db.Receipt.Find(ReceiptID);
                if (receipts != null)
                {
                    _db.Receipt.Remove(receipts);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Receipts with ID {ReceiptID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
