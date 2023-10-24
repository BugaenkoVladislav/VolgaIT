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
                var transports = db.Transports.Skip(start).Take(count).ToList();
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
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == id);
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
                User? user = db.Users.FirstOrDefault(x => transport.OwnerId == x.Id);
                if (user == null)
                   return NotFound("User with this id not found");
                if (transport.CanBeRented != true && transport.CanBeRented != false)
                    return NotFound("Not correct status CanBeRented");
                TransportType? transportType = db.TransportTypes.FirstOrDefault(x => transport.TransportType == x.TransportType1);
                if (transportType == null)
                    return NotFound("This transport type does not exist");
                Color? color = db.Colors.FirstOrDefault(x => transport.Color == x.Color1);
                if (color == null)
                    return NotFound("This color does not exist");
                Model? model = db.Models.FirstOrDefault(x => x.Model1 == transport.Model);
                if (model == null)
                    return NotFound("This model does not exist");
                db.Add(new Transport
                {                  
                    IdOwner = user.Id,
                    CanBeRented = (bool)transport.CanBeRented,
                    IdTransportType = transportType.Id,
                    IdModel = model.Id,
                    IdColor = color.Id,
                    Identifier = transport.Identifier,
                    Description = transport.Description,
                    Latitude = (double)transport.Latitude,
                    Longitude = (double)transport.Longitude,
                    MinutePrice = transport.MinutePrice,
                    DayPrice = transport.DayPrice,
                });
                db.SaveChanges();
                return Ok(db.TransportInfos.First(x => x.Identifier == transport.Identifier));
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
                if (owner == null)
                    return NotFound("User with this id not found");
                if (transport.CanBeRented != true && transport.CanBeRented != false)
                    return NotFound("Not correct status CanBeRented");
                TransportType? transportType = db.TransportTypes.FirstOrDefault(x => transport.TransportType == x.TransportType1);
                if (transport == null)
                    return NotFound("This transport type does not exist");
                Color? color = db.Colors.FirstOrDefault(x => transport.Color == x.Color1);
                if (color == null)
                    return NotFound("This color does not exist");
                Model? model = db.Models.FirstOrDefault(x => x.Model1 == transport.Model);
                if (model == null)
                    return NotFound("This model does not exist");
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
