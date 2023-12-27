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
    public class ServicesController : ControllerBase
    {
        private readonly PhotoContext _db;

        public ServicesController(PhotoContext db)
        {

            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.Services
                .Include(m => m.Group)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("GetbyServiceGroupID")]
        public JsonResult GetbyServiceGroupID(int ServiceGroupID)
        {
            var data = _db.Services
                .Include(m => m.Group)
                .Where(o => o.GroupID == ServiceGroupID)
                .ToList();
            return new JsonResult(data);
        }

        [HttpGet("Getservice")]
        public IActionResult GetserviceForFacilities()
        {
            try
            {
                // Lấy danh sách các cơ sở từ Claims trong HttpContext.User
                var facilityClaims = HttpContext.User.Claims.Where(c => c.Type == "FacilityName").Select(c => c.Value).ToList();

                if (facilityClaims.Any())
                {
                    // Tìm thông tin các cơ sở từ FacilityName
                    var facilities = _db.Facilities
                        .Include(f => f.Services) // Include để load thông tin services liên quan
                        .Where(f => facilityClaims.Contains(f.FacilityName))
                        .ToList();

                    var services = new List<Services>();

                    foreach (var facility in facilities)
                    {
                        // Thêm tất cả các services từ các cơ sở vào danh sách
                        services.AddRange(facility.Services);
                    }

                    return Ok(services);
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

        [HttpGet("ServiceNames")]
        public async Task<ActionResult> GetServiceAndGroupNames()
        {
            var serviceNames = await _db.Services
                .Where(s => s.ServiceID >= 1 && s.ServiceID <= 13) // Lọc theo ServiceID từ 1 đến 13
                .Include(s => s.Group) // Kết hợp Service và Group thông qua GroupID
                .Select(s => new
                {
                    GroupName = s.Group.GroupName,
                    ServiceName = s.ServiceName
                })
                .ToListAsync();

            return Ok(serviceNames);
        }




        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            var data = _db.Services

                .FirstOrDefault(m => m.ServiceID == id);
            return new JsonResult(data);
        }

        private const int WarningThreshold = 4500;
        private const int ResetThreshold = 5000;

        [HttpGet("service/total-quantity")]
        public IActionResult GetTotalQuantityForFacilities()
        {
            try
            {
                var facilityClaims = HttpContext.User.Claims.Where(c => c.Type == "FacilityName").Select(c => c.Value).ToList();

                if (!facilityClaims.Any())
                {
                    return BadRequest("Không có thông tin cơ sở được cung cấp.");
                }

                var facilityServiceQuantities = new Dictionary<string, Dictionary<string, int>>();

                foreach (var facilityClaim in facilityClaims)
                {
                    var servicesQuantities = new Dictionary<string, int>();

                    var facility = _db.Facilities
                        .FirstOrDefault(f => f.FacilityName == facilityClaim);

                    if (facility != null)
                    {
                        var services = _db.Services
                            .Where(s => s.FacilityID == facility.FacilityID)
                            .ToList();

                        foreach (var service in services)
                        {
                            var totalQuantity = _db.ReceiptDetail
                                .Where(rd => rd.Services.ServiceID == service.ServiceID)
                                .Sum(rd => rd.Quantity * rd.QuantitySets);

                            if (!servicesQuantities.ContainsKey(service.ServiceName))
                            {
                                servicesQuantities.Add(service.ServiceName, totalQuantity ?? 0);
                            }
                        }

                        facilityServiceQuantities.Add(facilityClaim, servicesQuantities);
                    }
                }

                return Ok(facilityServiceQuantities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


        //[HttpPost("service")]
        //public IActionResult CreateServiceOrder([FromBody] ServiceRequest serviceRequest)
        //{
        //    try
        //    {
        //        // Check ModelState validity
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest("Yêu cầu không hợp lệ.");
        //        }

        //        string userFacilityName = serviceRequest.Facility;

        //        // Find the user with the given username and corresponding facility
        //        var user = _db.Users.FirstOrDefault(u => u.UserName == serviceRequest.Username && u.UserFacilities.Any(uf => uf.Facility.FacilityName == userFacilityName));

        //        if (user == null)
        //        {
        //            return NotFound("Không tìm thấy người dùng phù hợp với cơ sở này.");
        //        }

        //        var selectedService = _db.Services.FirstOrDefault(s => s.ServiceID == serviceRequest.ServiceID);

        //        if (selectedService == null)
        //        {
        //            return NotFound("Không tìm thấy dịch vụ.");
        //        }

        //        // Assuming there is a one-to-many relationship between Facility and Services
        //        var facility = _db.Facilities.FirstOrDefault(f => f.FacilityName == userFacilityName);

        //        if (facility == null)
        //        {
        //            return BadRequest("Không tìm thấy cơ sở.");
        //        }

        //        // Assign the selected service to the facility
        //        facility.Services.Add(selectedService);
        //        _db.SaveChanges();

        //        return Ok("Dịch vụ đã được gán cho người dùng thành công.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}



        [HttpPost("Order")]
        public IActionResult CreateOrderForFacility([FromBody] ServiceCreationRequest serviceCreationRequest)
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
                        .Include(f => f.Services) // Include để load thông tin Services liên quan
                        .FirstOrDefault(f => f.FacilityName == facilityName);

                    if (facility != null)
                    {
                        // Tạo mới một photocopier dựa trên thông tin từ request
                        var newServices = new Services
                        {
                            FacilityID = facility.FacilityID,
                            GroupID = serviceCreationRequest.GroupID,
                            ServiceName = serviceCreationRequest.ServiceName,
                            Description = serviceCreationRequest.Description
                        };

                        // Lưu Services mới vào cơ sở dữ liệu của cơ sở đã đăng nhập
                        _db.Services.Add(newServices);
                        _db.SaveChanges();

                        return Ok(new
                        {
                            Message = $"Order Service đã được tạo thành công cho cơ sở {facilityName} này.",
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


        [HttpPost]
        public IActionResult Post(Services ser)
        {
            try
            {
                _db.Services.Add(ser);
                _db.SaveChanges();
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult UpdateSchool(int id, Services ser)
        {
            if (id != ser.ServiceID)
            {
                return BadRequest("Invalid ServiceID");
            }

            var existingServices = _db.Services.Find(id);
            if (existingServices is null)
            {
                return NotFound("School not found");
            }

            try
            {
                existingServices.ServiceName = ser.ServiceName;
                existingServices.Dvt = ser.Dvt;
                existingServices.Price = ser.Price;
                existingServices.Description = ser.Description;
                _db.Services.Update(existingServices);
                _db.SaveChanges();

                return Ok("Services updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update Services: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ServiceUpdateDto updateDto)
        {
            try
            {
                var service = _db.Services.Find(id);

                if (service == null)
                {
                    return NotFound();
                }

                service.ServiceUpdateDto(updateDto);

                _db.SaveChanges();

                return Ok(service);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("{ServiceID}")]
        public IActionResult Delete(int ServiceID)
        {
            try
            {
                var service = _db.Services.Find(ServiceID);
                if (service != null)
                {
                    _db.Services.Remove(service);
                    _db.SaveChanges();
                    return new JsonResult("Delete Successfully");
                }
                else
                {
                    return NotFound($"Service with ID {ServiceID} not found.");
                }
            }
            catch (Exception exc)
            {
                return BadRequest($"Error: {exc.Message}");
            }
        }
    }
}
