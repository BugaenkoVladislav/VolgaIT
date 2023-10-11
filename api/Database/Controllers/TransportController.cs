using api.Database.Json;
using api.Database.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

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
                Transport? transport = db.Transports.FirstOrDefault(t=> t.Id == id);
                if(transport is null) 
                {
                    return BadRequest();
                }
                return Ok(new JsonTransport
                {
                    CanBeRented = transport.CanBeRented,
                    TransportType = db.Transports.Include(o=> o.IdTransportTypeNavigation).First(x=> x.IdTransportType == transport.IdTransportType).IdTransportTypeNavigation.TransportType1,//c помощью навигационных средств ищем значения по другой таблице
                    Model = db.Transports.Include(o=> o.IdModelNavigation).First(x=> x.IdModel == transport.IdModel).IdModelNavigation.Model1,
                    Color = db.Transports.Include(o=> o.IdColorNavigation).First(x=> x.IdColor == transport.IdColor).IdColorNavigation.Color1,
                    Identifier = transport.Identifier,
                    Latitude = transport.Latitude,
                    Longitude = transport.Longitude,
                    MinutePrice = transport.MinutePrice,
                    DayPrice = transport.DayPrice,
                    Description = transport.Description,
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }


        [Authorize]
        [HttpPost("/Transport")]
        public IActionResult AddTransport([FromBody] JsonTransport transport)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                db.Add(new Transport
                {
                    CanBeRented = transport.CanBeRented,
                    IdTransportType = db.TransportTypes.First(x => x.TransportType1.ToLower() == transport.TransportType.ToLower()).Id,
                    IdModel = db.Models.First(x => x.Model1.ToLower() == transport.Model.ToLower()).Id,
                    IdColor = db.Colors.First(x => x.Color1.ToLower() == transport.Color.ToLower()).Id,
                    Identifier = transport.Identifier,
                    Latitude = transport.Latitude,
                    Longitude = transport.Longitude,
                    MinutePrice = transport.MinutePrice,
                    DayPrice = transport.DayPrice,
                    Description = transport.Description,
                    IdOwner = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt)).Id,
                    IdColorNavigation = db.Colors.First(x => x.Color1.ToLower() == transport.Color.ToLower()),
                    IdModelNavigation = db.Models.First(x => x.Model1.ToLower() == transport.Model.ToLower()),
                    IdOwnerNavigation = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt)),
                    IdTransportTypeNavigation = db.TransportTypes.First(x => x.TransportType1.ToLower() == transport.TransportType.ToLower())
                });
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
