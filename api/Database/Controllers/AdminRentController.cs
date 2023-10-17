using api.Database.Models;
using api.Database.Views;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace api.Database.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api")]
    [ApiController]
    public class AdminRentController : ControllerBase
    {
        SimbirGoContext db;
        public AdminRentController(SimbirGoContext db)
        {
            this.db = db;
        }
        [HttpGet("/api/Admin/Rent/{rentId}")]
        public IActionResult GetRent([FromRoute] int rentId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                RentInfo? rent = db.RentInfos.FirstOrDefault(x => x.Id == rentId);
                if (rent == null)
                    return NotFound("Id not exist");
                return Ok(rent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/Admin/UserHistory/{userId}")]
        public IActionResult GetUserRents([FromRoute] long userId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                User? user = db.Users.FirstOrDefault(x => x.Id == userId);
                if(user == null)
                    return NotFound("User with this Id not exist");
                var rent = db.RentInfos.Where(x => x.UserId == user.Id).ToList();
                return Ok(rent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/Admin/transportHistory/{transportId}")]
        public IActionResult GettransportRents([FromRoute] long transportId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == transportId);
                if (transport == null)
                    return NotFound("Transport with this Id not exist");
                var rent = db.RentInfos.Where(x => x.TransportId == transportId).ToList();
                return Ok(rent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("/api/Admin/Rent")]
        public IActionResult NewRent([FromBody] RentInfo rent )//подчисти мусор
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                User? user = db.Users.FirstOrDefault(x=> x.Id == rent.UserId);
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == rent.TransportId);                
                if (user == null)
                    return NotFound("User with this ID does not exist");
                if(transport == null || transport.CanBeRented == false)
                    return NotFound("Transport with this ID does not exist");
                RentType? priceType = db.RentTypes.FirstOrDefault(x => x.RentType1 == rent.PriceType);
                if (priceType == null)
                    return NotFound("This type does not exist");
                db.Add(new Rent
                {                    
                    IdTransport = Convert.ToInt64(transport.Id),
                    IdUser = Convert.ToInt64(user.Id),
                    TimeStart = Convert.ToDateTime(rent.TimeStart),
                    TimeEnd = rent.TimeEnd,                          
                    PriceType = priceType.Id,
                    PriceOfUnit = Convert.ToDouble(rent.PriceOfUnit),
                    FinalPrice = rent.FinalPrice
                });
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("/api/Admin/Rent/{id}")]
        public IActionResult UpdateRent([FromRoute] int id ,[FromBody] RentInfo rentInfo)//подчисти мусор
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                Rent? rent = db.Rents.FirstOrDefault(x => x.Id == id);
                User? user = db.Users.FirstOrDefault(x => x.Id == rentInfo.UserId);
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == rentInfo.TransportId);
                if (rent == null)
                    return NotFound("Rent with this ID does not exist");
                if (user == null)
                    return NotFound("User with this ID does not exist");
                if (transport == null)
                    return NotFound("Transport with this ID does not exist");
                RentType? priceType = db.RentTypes.FirstOrDefault(x => x.RentType1 == rentInfo.PriceType);
                if (priceType == null)
                    return NotFound("this PriceType with this ID does not exist");
                rent.IdTransport = Convert.ToInt64(transport.Id);
                rent.IdUser = Convert.ToInt64(user.Id);
                rent.TimeStart = Convert.ToDateTime(rentInfo.TimeStart);
                rent.TimeEnd = rentInfo.TimeEnd;
                rent.PriceType = priceType.Id;
                rent.PriceOfUnit = Convert.ToDouble(rent.PriceOfUnit);
                rent.FinalPrice = rentInfo.FinalPrice;
                db.Update(rent);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/api/Admin/Rent/{rentId}")]
        public IActionResult DeleteRent([FromRoute] int rentId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                Rent? rent = db.Rents.FirstOrDefault(x => x.Id == rentId);
                if (rent == null)
                    return NotFound("Id not exist");
                db.Remove(rent);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/api/Admin/Rent/End/{rentId}")]
        public IActionResult EndRent([FromRoute] int rentId,double lat, double @long)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? usr = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (usr == null)
                    return NotFound("Uncorrect token");
                Rent? rent = db.Rents.FirstOrDefault(x => x.Id == rentId);
                if (rent != null)
                {
                    Transport transport = db.Transports.First(x => x.Id == rent.IdTransport);                    
                    rent.TimeEnd = DateTime.UtcNow;
                    TimeSpan difference = DateTime.UtcNow - rent.TimeStart;
                    if (rent.PriceType == 2)
                    {
                         rent.FinalPrice = difference.Days + 1 * rent.PriceOfUnit;
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
                return NotFound("Rent with this Id does not exist");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
