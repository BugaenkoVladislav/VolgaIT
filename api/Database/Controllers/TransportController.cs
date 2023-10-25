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
    [Route("api")]
    [ApiController]
    public class TransportController : ControllerBase
    {
        SimbirGoContext db;
        public TransportController(SimbirGoContext db) 
        {            
            this.db = db;
        }


        [HttpGet("/api/Transport/{id}")]
        public IActionResult GetTransport([FromRoute] int id)
        {
            try
            {
                TransportInfo? transportInfo = db.TransportInfos.FirstOrDefault(x=>x.Id == id);
                if (transportInfo is null)
                {
                    return NotFound("Transport with this id not exist");
                }
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
        [HttpPost("/api/Transport")]
        public IActionResult AddTransport([FromBody] TransportInfo transportInfo)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user is null)
                    return NotFound("Uncorrect Token");
                db.Add(new Transport
                {                   
                    CanBeRented = (bool)transportInfo.CanBeRented,
                    IdTransportType = db.TransportTypes.First(x => x.TransportType1 == transportInfo.TransportType).Id,
                    IdModel = db.Models.First(x => x.Model1 == transportInfo.Model).Id,
                    IdColor = db.Colors.First(x => x.Color1 == transportInfo.Color).Id,
                    Identifier = transportInfo.Identifier,
                    Latitude = Convert.ToDouble(transportInfo.Latitude),
                    Longitude = Convert.ToDouble(transportInfo.Longitude),
                    MinutePrice = Convert.ToDouble(transportInfo.MinutePrice),
                    DayPrice = Convert.ToDouble(transportInfo.DayPrice),
                    Description = transportInfo.Description,
                    IdOwner = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt)).Id
                });
                db.SaveChanges();                
                return Ok(db.TransportInfos.First(x=>x.Identifier == transportInfo.Identifier));
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch(Exception ex) 
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpPut("/api/Transport/{id}")]
        public IActionResult UpdateTransport([FromRoute] int id, [FromBody] TransportInfo transportInfo )
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");//получаем наш jwt токен
                User? user = db.Users.FirstOrDefault(x=> x.Username == JwtActions.ReturnUsername(jwt));
                if (user is null)
                    return NotFound("Uncorrect Token");
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == id);
                if (transport.IdOwner != user.Id)
                    return Forbid();
                transport.CanBeRented = (bool)transportInfo.CanBeRented;                
                transport.IdModel = db.Models.First(x => x.Model1 == transportInfo.Model).Id;
                transport.IdColor = db.Colors.First(x => x.Color1 == transportInfo.Color).Id;
                transport.Identifier = transportInfo.Identifier;
                transport.Latitude = Convert.ToDouble(transportInfo.Latitude);
                transport.Longitude = Convert.ToDouble(transportInfo.Longitude);
                transport.MinutePrice = Convert.ToDouble(transportInfo.MinutePrice);
                transport.DayPrice = Convert.ToDouble(transportInfo.DayPrice);
                transport.Description = transportInfo.Description;
                transport.IdOwner = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt)).Id;
                
                db.Update(transport);
                db.SaveChanges();
                return Ok(transport);

            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("/api/Transport/{id}")]
        public IActionResult DeleteTransport([FromRoute]int id)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect Token");
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == id);
                if (transport == null || user.Id != transport.IdOwner)
                    return Forbid();
                db.Remove(transport);
                db.SaveChanges();
                return Ok();    
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

    }
}
