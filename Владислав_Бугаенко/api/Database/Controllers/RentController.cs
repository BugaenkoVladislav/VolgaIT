using api.Database.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace api.Database.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class RentController : ControllerBase
    {
        private SimbirGoContext _db;
        private HttpRequest _request;
        public RentController(SimbirGoContext db,IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _request = httpContextAccessor.HttpContext.Request;
        }


        [HttpGet("Transport")]
        public IActionResult RentTransportRadius(double lat, double @long, double radius, string type)
        {
            var result = new List<Transport>();
            try
            {
                var transportType = _db.TransportTypes.FirstOrDefault(x => x.TransportType1 == type);
                if (transportType == null && type != "All")
                    return BadRequest("not have this type");
                if (transportType != null)
                {
                    var ts = _db.Transports.Where(x => x.IdTransportType == transportType.Id).ToList();
                    result = JwtActions.FindInRadius(ts, lat, @long, radius);
                }
                else
                {
                    var ts = _db.Transports.ToList();
                    result = JwtActions.FindInRadius(ts, lat, @long, radius);
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{rentId}")]
        public IActionResult GetRentInfo([FromRoute] int rentId)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                var rent = _db.RentInfos.FirstOrDefault(x => x.Id == rentId);
                if (user.Id == rent.UserId || user.Id == rent.OwnerId)
                    return Ok(rent);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpGet("MyHistory")]
        public IActionResult GetUserRentInfo()
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                var rents = _db.RentInfos.Where(x => x.UserId == user.Id).ToList();
                return Ok(rents);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("TransportHistory/{transportId}")]
        public IActionResult GetUserRentInfo([FromRoute] int transportId)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                var transport = _db.Transports.FirstOrDefault(x => x.Id == transportId);
                if (transport.IdOwner == user.Id || transport is null)
                {
                    var rentInfos = _db.RentInfos.Where(x => x.TransportId == transportId).ToList();
                    return Ok(rentInfos);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("New/{transportId}")]
        public IActionResult RentNew([FromRoute] long transportId, string rentType)//подчисти мусор
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                var ts = _db.Transports.FirstOrDefault(x => x.Id == transportId);
                if(ts.IdOwner == user.Id)
                    return Forbid();
                var priceType = _db.RentTypes.FirstOrDefault(x => x.RentType1 == rentType);
                double priceOfUnit=0;
                switch (priceType.Id)
                {
                    case 1:
                        priceOfUnit = Convert.ToDouble(_db.Transports.First(x => x.Id == transportId).MinutePrice);
                        break;
                    case 2:
                        priceOfUnit = Convert.ToDouble(_db.Transports.First(x => x.Id == transportId).DayPrice);
                        break;
                }
                _db.Add(new Rent
                {
                    TimeStart = DateTime.UtcNow,
                    IdUser = user.Id,
                    IdTransport = transportId,
                    PriceType = priceType.Id,
                    PriceOfUnit = priceOfUnit
                });
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("End/{rentId}")]
        public IActionResult RentEnd([FromRoute] long rentId, double lat, double @long)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                var rent = _db.Rents.FirstOrDefault(x => x.Id == rentId);
                var transport = _db.Transports.First(x => x.Id == rent.IdTransport);
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
                    _db.Update(rent);
                    transport.Latitude = lat;
                    transport.Longitude = @long;
                    _db.Update(transport);
                    _db.SaveChanges();
                    return Ok();
                }
                return Forbid("not your rent");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
