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


        [HttpGet("/Transport/{id}")]
        public IActionResult GetTransport([FromRoute] int id)
        {
            try
            {
                TransportInfo? transportInfo = db.TransportInfos.First(x=>x.Id == id);
                if(transportInfo is null) 
                {
                    return BadRequest();
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
        [HttpPost("/Transport")]
        public IActionResult AddTransport([FromBody] TransportInfo transportInfo)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
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
                    IdOwner = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt)).Id,
                    IdOwnerNavigation = db.Users.First(x=>x.Username == JwtActions.ReturnUsername(jwt)),
                    IdColorNavigation = db.Colors.First(x=>x.Color1 == transportInfo.Color),
                    IdModelNavigation = db.Models.First(x => x.Model1 == transportInfo.Model)
                });
                db.SaveChanges();
                
                return Ok();
            }
            catch(Exception ex) 
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpPut("/Transport/{id}")]
        public IActionResult UpdateTransport([FromRoute] int id, [FromBody] TransportInfo transportInfo )
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");//получаем наш jwt токен
                User? user = db.Users.FirstOrDefault(x=> x.Username == JwtActions.ReturnUsername(jwt));
                if (user is null)
                    return StatusCode(401);
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == id);
                if (transport == null || transport.IdOwner != user.Id)
                    return BadRequest("not owner");
                transport.CanBeRented = (bool)transportInfo.CanBeRented;
                transport.IdTransportType = db.TransportTypes.First(x => x.TransportType1 == transportInfo.TransportType).Id;
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
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("/Transport/{id}")]
        public IActionResult DeleteTransport([FromRoute]int id)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == id);
                if (transport == null || user.Id != transport.IdOwner)
                    return BadRequest("not owner ");
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
