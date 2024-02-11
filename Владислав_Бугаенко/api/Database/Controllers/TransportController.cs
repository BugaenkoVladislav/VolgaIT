using api.Database.Models;
using api.Database.Views;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Cryptography.Xml;

namespace api.Database.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class TransportController : ControllerBase
    {
        private HttpRequest _request;
        private SimbirGoContext _db;
        public TransportController(SimbirGoContext db, IHttpContextAccessor httpContextAccessor) 
        {            
            _db = db;
            _request = httpContextAccessor.HttpContext.Request;
        }


        [HttpGet("{id}")]
        public IActionResult GetTransport([FromRoute] int id)
        {
            try
            {
                var transportInfo = _db.TransportInfos.FirstOrDefault(x=>x.Id == id);
                if (transportInfo is null)
                    return NotFound("Transport with this id not exist");
                return Ok(new TransportInfo
                {
                    Id = transportInfo.Id,
                    CanBeRented = transportInfo.CanBeRented,
                    TransportType = transportInfo.TransportType,
                    Model = transportInfo.Model,
                    Color = transportInfo.Color,
                    Identifier = transportInfo.Identifier,
                    Description = transportInfo.Description,
                    Latitude = transportInfo.Latitude,
                    Longitude = transportInfo.Longitude,
                    MinutePrice = transportInfo.MinutePrice,
                    DayPrice = transportInfo.DayPrice
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [Authorize]
        [HttpPost("")]
        public IActionResult AddTransport([FromBody] TransportInfo transportInfo)
        {
            try
            {
                _db.Add(new Transport
                {
                    CanBeRented = (bool)transportInfo.CanBeRented,
                    IdTransportType = _db.TransportTypes.First(x => x.TransportType1 == transportInfo.TransportType).Id,
                    IdModel = _db.Models.First(x => x.Model1 == transportInfo.Model).Id,
                    IdColor = _db.Colors.First(x => x.Color1 == transportInfo.Color).Id,
                    Identifier = transportInfo.Identifier,
                    Latitude = Convert.ToDouble(transportInfo.Latitude),
                    Longitude = Convert.ToDouble(transportInfo.Longitude),
                    MinutePrice = Convert.ToDouble(transportInfo.MinutePrice),
                    DayPrice = Convert.ToDouble(transportInfo.DayPrice),
                    Description = transportInfo.Description,
                    IdOwner = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request)).Id
                });
                _db.SaveChanges();
                return Ok(_db.TransportInfos.First(x => x.Identifier == transportInfo.Identifier));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex) 
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateTransport([FromRoute] int id, [FromBody] TransportInfo transportInfo )
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                var transport = _db.Transports.FirstOrDefault(x => x.Id == id);
                if (transport.IdOwner == user.Id)
                {
                    transport.CanBeRented = (bool)transportInfo.CanBeRented;
                    transport.IdModel = _db.Models.First(x => x.Model1 == transportInfo.Model).Id;
                    transport.IdColor = _db.Colors.First(x => x.Color1 == transportInfo.Color).Id;
                    transport.Identifier = transportInfo.Identifier;
                    transport.Latitude = Convert.ToDouble(transportInfo.Latitude);
                    transport.Longitude = Convert.ToDouble(transportInfo.Longitude);
                    transport.MinutePrice = Convert.ToDouble(transportInfo.MinutePrice);
                    transport.DayPrice = Convert.ToDouble(transportInfo.DayPrice);
                    transport.Description = transportInfo.Description;
                    transport.IdOwner = _db.Users.First(x => x.Username == JwtActions.ReturnUsername(_request)).Id;
                    _db.Update(transport);
                    _db.SaveChanges();
                    return Ok(transport);
                }
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteTransport([FromRoute]int id)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                var transport = _db.Transports.FirstOrDefault(x => x.Id == id);
                if (transport == null || user.Id != transport.IdOwner)
                    return Forbid();
                _db.Remove(transport);
                _db.SaveChanges();
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

    }
}
