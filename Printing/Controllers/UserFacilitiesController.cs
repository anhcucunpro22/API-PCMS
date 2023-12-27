using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Printing.Data;

namespace Printing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFacilitiesController : ControllerBase
    {
        private readonly PhotoContext _db;
        public UserFacilitiesController(PhotoContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var data = _db.UserFacilities
                .Include(m => m.User)
                .Include(m => m.Facility)
                .ToList();
            return new JsonResult(data);
        }
    }
}
