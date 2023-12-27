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
    public class PhotocopierController : ControllerBase
    {
        private readonly PhotoContext _db;

        public PhotocopierController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Photocopier
                .Include(m => m.Facility)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("Getbyfacilities")]
        public JsonResult Getbyfacilities(int FacilityID)
        {
            var data = _db.Photocopier
                .Include(m => m.Facility)
                .Where(o => o.FacilityID == FacilityID)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("photocopiers")]
        public IActionResult GetPhotocopiersForFacilities()
        {
            try
            {
                // Lấy danh sách các cơ sở từ Claims trong HttpContext.User
                var facilityClaims = HttpContext.User.Claims.Where(c => c.Type == "FacilityName").Select(c => c.Value).ToList();

                if (facilityClaims.Any())
                {
                    // Tìm thông tin các cơ sở từ FacilityName
                    var facilities = _db.Facilities
                        .Include(f => f.Photocopiers) // Include để load thông tin Photocopiers liên quan
                        .Where(f => facilityClaims.Contains(f.FacilityName))
                        .ToList();

                    var photocopiers = new List<Photocopier>();

                    foreach (var facility in facilities)
                    {
                        // Thêm tất cả các máy photocopy từ các cơ sở vào danh sách
                        photocopiers.AddRange(facility.Photocopiers);
                    }

                    return Ok(photocopiers);
                }
                else
                {
                    return BadRequest("Không có thông tin cơ sở được cung cấp.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Photocopier
                .Include(m => m.Facility)
                .FirstOrDefault(m => m.PhotocopierID == id);
            return new JsonResult(data);
        }

        [HttpPost]
        public IActionResult Post(Photocopier phoc)
        {
            try
            {
                _db.Photocopier.Add(phoc);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("photocopiers")]
        public IActionResult CreatePhotocopierForFacility([FromBody] PhotocopierCreationRequest photocopierRequest)
        {
            try
            {
                // Lấy thông tin cơ sở từ Claims trong HttpContext.User
                var facilityNameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "FacilityName");

                if (facilityNameClaim != null)
                {
                    string facilityName = facilityNameClaim.Value;

                    // Tìm thông tin cơ sở từ FacilityName
                    var facility = _db.Facilities
                        .Include(f => f.Photocopiers) // Include để load thông tin Photocopiers liên quan
                        .FirstOrDefault(f => f.FacilityName == facilityName);

                    if (facility != null)
                    {
                        // Tạo mới một photocopier dựa trên thông tin từ request
                        var newPhotocopier = new Photocopier
                        {
                            FacilityID = facility.FacilityID,
                            PhotocopierName = photocopierRequest.PhotocopierName,
                            Description = photocopierRequest.Description,
                            SerialNumber = photocopierRequest.SerialNumber,
                            Location = photocopierRequest.Location,
                            IsActive = photocopierRequest.IsActive,
                            Notes = photocopierRequest.Notes
                        };

                        // Lưu photocopier mới vào cơ sở dữ liệu của cơ sở đã đăng nhập
                        _db.Photocopier.Add(newPhotocopier);
                        _db.SaveChanges();

                        return Ok(new
                        {
                            Message = $"Photocopier đã được tạo thành công cho cơ sở {facilityName} này.",
                            FacilityName = facilityName
                        });
                    }
                    else
                    {
                        return NotFound("Không tìm thấy thông tin cho cơ sở này.");
                    }
                }
                else
                {
                    return BadRequest("Không có thông tin cơ sở được cung cấp.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        public IActionResult UpdateSchool(int id, Photocopier phoc)
        {
            if (id != phoc.PhotocopierID)
            {
                return BadRequest("Invalid PhotocopierID");
            }

            var existingPhotocopier = _db.Photocopier.Find(id);
            if (existingPhotocopier is null)
            {
                return NotFound("Photocopier not found");
            }

            try
            {
                existingPhotocopier.PhotocopierName = phoc.PhotocopierName;
                existingPhotocopier.Description = phoc.Description;
                existingPhotocopier.SerialNumber = phoc.SerialNumber;
                existingPhotocopier.Location = phoc.Location;
                existingPhotocopier.IsActive = phoc.IsActive;
                existingPhotocopier.Notes = phoc.Notes;

                _db.Photocopier.Update(existingPhotocopier);
                _db.SaveChanges();

                return Ok("Photocopier updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Photocopier: {ex.Message}");
            }
        }

        [HttpDelete("{PhotocopierID}")]
        public IActionResult Delete(int PhotocopierID)
        {
            try
            {
                var photocopier = _db.Photocopier.Find(PhotocopierID);
                if (photocopier != null)
                {
                    _db.Photocopier.Remove(photocopier);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Photocopier with ID {PhotocopierID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
