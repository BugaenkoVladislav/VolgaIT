using api.Database.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                return Ok(transport);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [Authorize]
        [HttpPost("/Transport")]
        public IActionResult AddTransport([FromBody] Transport transport)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                db.Add(new Transport
                {
                    CanBeRented = transport.CanBeRented,
                    IdTransportType = db.TransportTypes.First(u=>u.TransportType1 == Convert.ToString(transport.IdTransportType)).Id,//to int
                    IdModel = db.Models.First(u => u.Model1 == Convert.ToString(transport.IdModel)).Id,//to int 
                    IdColor = db.Colors.First(u => u.Color1 == Convert.ToString(transport.IdColor)).Id,// Toint
                    Identifier = transport.Identifier,
                    Description = transport.Description,
                    Latitute = transport.Latitute,
                    Longitude = transport.Longitude,
                    MinutePrice = transport.MinutePrice,
                    DayPrice = transport.DayPrice,
                    IdOwner = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt)).Id,
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
        [HttpDelete("/Transport/{id}")]
        public IActionResult DeleteTransport([FromRoute] int id)
        {
            try
            {
                
                Transport? transport = db.Transports.FirstOrDefault(t => t.Id == id);
                if (transport is null)
                {
                    return BadRequest();
                }
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (transport.IdOwner == user.Id)
                {
                    db.Remove(transport);
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpPut("/Transport/{id}")]
        public IActionResult UpdateTransport([FromRoute] int id, [FromBody] Transport transport)
        {
            try
            {

                Transport? thisTransport = db.Transports.FirstOrDefault(t => t.Id == id);
                if (transport is null)
                {
                    return BadRequest();
                }
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (thisTransport.IdOwner == user.Id)
                {
                    thisTransport = transport;
                    db.Update(thisTransport);
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
