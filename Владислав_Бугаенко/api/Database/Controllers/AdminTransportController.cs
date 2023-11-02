using api.Database.Models;
using api.Database.Views;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Database.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api")]
    [ApiController]
    public class AdminTransportController : ControllerBase
    {

        SimbirGoContext db;
        public AdminTransportController(SimbirGoContext db)
        {
            this.db = db;
        }

        [HttpGet("/api/Admin/Transport")]
        public IActionResult GetAllTransports(int start, int count)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect token");
                var transports = db.TransportInfos.Skip(start).Take(count).ToList();
                return Ok(transports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/Admin/Transport/{id}")]
        public IActionResult GetTransportById([FromRoute] int id)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                TransportInfo? transport = db.TransportInfos.FirstOrDefault(x => x.Id == id);
                if (transport == null)
                    return NotFound("Id not exist");
                return Ok(transport);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("/api/Admin/Transport")]
        public IActionResult AddNewTransport([FromBody] TransportInfo transport)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                Transport? t = db.Transports.FirstOrDefault(x => x.Identifier == transport.Identifier);
                if (t != null)
                    return BadRequest("Transport with this Identifier already exist");
                db.Add(new Transport
                {
                    IdOwner = Convert.ToInt64(transport.OwnerId),
                    CanBeRented = (bool)transport.CanBeRented,
                    IdTransportType = db.TransportTypes.First(x => x.TransportType1 == transport.TransportType).Id,
                    IdModel = db.Models.First(x => x.Model1 == transport.Model).Id,
                    IdColor = db.Colors.First(x => x.Color1 == transport.Color).Id,
                    Identifier = transport.Identifier,
                    Latitude = Convert.ToDouble(transport.Latitude),
                    Longitude = Convert.ToDouble(transport.Longitude),
                    MinutePrice = Convert.ToDouble(transport.MinutePrice),
                    DayPrice = Convert.ToDouble(transport.DayPrice),
                    Description = transport.Description,
                });
                db.SaveChanges();
                return Ok(db.TransportInfos.First(x => x.Identifier == transport.Identifier));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }
        [HttpPut("/api/Admin/Transport/{id}")]
        public IActionResult UpdateTransport([FromRoute] int id, [FromBody] TransportInfo transport)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                Transport? t = db.Transports.FirstOrDefault(x => x.Id == id);
                if (t == null)
                    return NotFound("Transport with this Id does not exist");
                User? owner = db.Users.FirstOrDefault(x => transport.OwnerId == x.Id);
                TransportType? transportType = db.TransportTypes.FirstOrDefault(x => transport.TransportType == x.TransportType1);
                Color? color = db.Colors.FirstOrDefault(x => transport.Color == x.Color1);
                Model? model = db.Models.FirstOrDefault(x => x.Model1 == transport.Model);
                t.IdOwner = owner.Id;
                t.CanBeRented = (bool)transport.CanBeRented;
                t.IdTransportType = transportType.Id;
                t.IdModel = model.Id;
                t.IdColor = color.Id;
                t.Identifier = transport.Identifier;
                t.Description = transport.Description;
                t.Latitude = (double)transport.Latitude;
                t.Longitude = (double)transport.Longitude;
                t.MinutePrice = transport.MinutePrice;
                t.DayPrice = transport.DayPrice;
                db.Update(t);
                db.SaveChanges();
                return Ok(t);
            }
            catch(NullReferenceException)
            {
                return BadRequest();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("/api/Admin/Transport/{id}")]
        public IActionResult DeleteTransport([FromRoute] int id)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                Transport? t = db.Transports.FirstOrDefault(x => x.Id == id);
                if (t == null)
                    return NotFound("Transport with this Id does not exist");
                db.Remove(t);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
