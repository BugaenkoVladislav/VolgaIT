using api.Database.Models;
using api.Database.Views;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Database.Controllers
{
    [Route("api")]
    [ApiController]
    public class RentController : ControllerBase
    {
        SimbirGoContext db;
        public RentController(SimbirGoContext db) 
        {
            this.db = db;
        }


        [HttpGet("/Rent/Transport")]
        public IActionResult RentTransportRadius(double lat, double @long, double radius)
        {
            try
            {
                List<long> tsId = new List<long>();
                foreach(var t in db.Transports)
                {
                    double d = Math.Sqrt(Math.Pow(Convert.ToDouble(t.Longitude) - @long, 2) + Math.Pow(Convert.ToDouble(t.Latitude) - lat, 2));
                    if (d <= radius)
                        tsId.Add(t.Id);
                }
                var transports = db.TransportInfos.Where(u => tsId.Contains((u.Id.Value))).ToList();
                return Ok(transports);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpGet("/Rent/{rentId}")]
        public IActionResult GetRentInfo([FromRoute] int rentId)
        {
            try
            {               
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }


    }
}
