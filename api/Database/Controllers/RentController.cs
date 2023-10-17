using api.Database.Models;
using api.Database.Views;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        [HttpGet("/api/Rent/Transport")]
        public IActionResult RentTransportRadius(double lat, double @long, double radius, string type)
        {
            try
            {
                TransportType? transportType = db.TransportTypes.FirstOrDefault(x => x.TransportType1 == type);
                if (transportType == null)
                    return BadRequest("not have this type");
                List<long> tsId = new List<long>();
                foreach (var t in db.Transports.Where(x=>x.IdTransportType == transportType.Id).ToList())
                {
                    double d = Math.Sqrt(Math.Pow(Convert.ToDouble(t.Longitude) - @long, 2) + Math.Pow(Convert.ToDouble(t.Latitude) - lat, 2));
                    if (d <= radius)
                        tsId.Add(t.Id);
                }
                var transports = db.TransportInfos.Where(u => tsId.Contains(u.Id.Value) && u.CanBeRented == true).ToList();
                return Ok(transports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("/api/Rent/{rentId}")]
        public IActionResult GetRentInfo([FromRoute] int rentId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                RentInfo? rent = db.RentInfos.FirstOrDefault(x => x.Id == rentId);
                if(user == null)
                    return NotFound("Uncorrect Token");
                if (rent == null)
                    return NotFound("id with this rent not exist");
                if (user.Id != rent.UserId || user.Id != rent.OwnerId)
                    return Ok(rent);
                return BadRequest();
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpGet("/api/Rent/MyHistory")]
        public IActionResult GetUserRentInfo()//works but i wanna some remade RentInfo and Rents
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect Token ");
                List<RentInfo> rents = db.RentInfos.Where(x => x.UserId == user.Id).ToList();
                return Ok(rents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("/api/Rent/TransportHistory/{transportId}")]
        public IActionResult GetUserRentInfo([FromRoute] int transportId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect Token");
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == transportId);
                if (transport is null)
                    return NotFound("uncorrect Id transport");
                if (transport.IdOwner == user.Id)
                {
                    List<RentInfo> rentInfos = db.RentInfos.Where(x => x.TransportId == transportId).ToList();
                    return Ok(rentInfos);
                }
                return Forbid("not owner");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("/api/Rent/New/{transportId}")]
        public IActionResult RentNew([FromRoute] long transportId, string rentType)//подчисти мусор
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect Token");
                Transport? ts = db.Transports.FirstOrDefault(x => x.Id == transportId);
                if (ts == null || ts.CanBeRented == false)
                    return NotFound("Transport with this ID does not exist");
                if(ts.IdOwner == user.Id)
                    return Forbid("owner can not rent him transport");
                
                RentType? priceType = db.RentTypes.FirstOrDefault(x => x.RentType1 == rentType);
                if (priceType == null)
                    return BadRequest("not have this type");
                double priceOfUnit=0;
                switch (priceType.Id)
                {
                    case 1:
                        priceOfUnit = Convert.ToDouble(db.Transports.First(x => x.Id == transportId).MinutePrice);
                        break;
                    case 2:
                        priceOfUnit = Convert.ToDouble(db.Transports.First(x => x.Id == transportId).DayPrice);
                        break;                        
                }

                db.Add(new Rent
                {
                    TimeStart = DateTime.UtcNow,
                    IdUser = user.Id,
                    IdTransport = transportId,
                    PriceType = priceType.Id,
                    PriceOfUnit = priceOfUnit
                });
                db.SaveChanges();
                return Ok();                               
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("/api/Rent/End/{rentId}")]
        public IActionResult RentEnd([FromRoute] long rentId, double lat, double @long)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect Token");
                Rent? rent = db.Rents.FirstOrDefault(x => x.Id == rentId);
                if(rent != null)
                {
                    Transport transport = db.Transports.First(x => x.Id == rent.IdTransport);
                    if (rent.IdUser == user.Id)
                    {
                        rent.TimeEnd = DateTime.UtcNow;
                        TimeSpan difference = DateTime.UtcNow - rent.TimeStart;
                        if (rent.PriceType == 2)
                        {
                            rent.FinalPrice = (difference.Days + 1) * rent.PriceOfUnit;
                        }
                        else if (rent.PriceType == 1)
                        {
                            rent.FinalPrice = (difference.Minutes + (difference.Hours * 60) + (difference.Days * 24 * 60)) * rent.PriceOfUnit;
                        }
                        db.Update(rent);
                        transport.Latitude = lat;
                        transport.Longitude = @long;
                        db.Update(transport);
                        db.SaveChanges();
                        return Ok();
                    }
                    return Forbid("not your rent");
                }
                return NotFound("Rent with this Id does not exist");               
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
