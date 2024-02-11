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
    [Route("api/[controller]/")]
    [ApiController]
    public class AdminTransportController : ControllerBase
    {
        SimbirGoContext _db;
        public AdminTransportController(SimbirGoContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public IActionResult GetAllTransports(int start, int count)
        {
            try
            {
                var transports = _db.TransportInfos.Skip(start).Take(count).ToList();
                return Ok(transports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetTransportById([FromRoute] int id)
        {
            try
            {
                var transport = _db.TransportInfos.FirstOrDefault(x => x.Id == id);
                if (transport == null)
                    return NotFound("Id not exist");
                return Ok(transport);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpPost("")]
        public IActionResult AddNewTransport([FromBody] TransportInfo transport)
        {
            try
            {
                var t = _db.Transports.FirstOrDefault(x => x.Identifier == transport.Identifier);
                if (t != null)
                    return BadRequest("Transport with this Identifier already exist");
                _db.Add(new Transport
                {
                    IdOwner = Convert.ToInt64(transport.OwnerId),
                    CanBeRented = (bool)transport.CanBeRented,
                    IdTransportType = _db.TransportTypes.First(x => x.TransportType1 == transport.TransportType).Id,
                    IdModel = _db.Models.First(x => x.Model1 == transport.Model).Id,
                    IdColor = _db.Colors.First(x => x.Color1 == transport.Color).Id,
                    Identifier = transport.Identifier,
                    Latitude = Convert.ToDouble(transport.Latitude),
                    Longitude = Convert.ToDouble(transport.Longitude),
                    MinutePrice = Convert.ToDouble(transport.MinutePrice),
                    DayPrice = Convert.ToDouble(transport.DayPrice),
                    Description = transport.Description,
                });
                _db.SaveChanges();
                return Ok(_db.TransportInfos.First(x => x.Identifier == transport.Identifier));
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

        [HttpPut("{id}")]
        public IActionResult UpdateTransport([FromRoute] int id, [FromBody] TransportInfo transport)
        {
            try
            {
                var t = _db.Transports.FirstOrDefault(x => x.Id == id);
                if (t == null)
                    return NotFound("Transport with this Id does not exist");
                var owner = _db.Users.FirstOrDefault(x => transport.OwnerId == x.Id);
                var transportType = _db.TransportTypes.FirstOrDefault(x => transport.TransportType == x.TransportType1);
                var color = _db.Colors.FirstOrDefault(x => transport.Color == x.Color1);
                var model = _db.Models.FirstOrDefault(x => x.Model1 == transport.Model);
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
                _db.Update(t);
                _db.SaveChanges();
                return Ok(t);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
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
                
        [HttpDelete("{id}")]
        public IActionResult DeleteTransport([FromRoute] int id)
        {
            try
            {
                var t = _db.Transports.FirstOrDefault(x => x.Id == id);
                if (t == null)
                    return NotFound("Transport with this Id does not exist");
                _db.Remove(t);
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
