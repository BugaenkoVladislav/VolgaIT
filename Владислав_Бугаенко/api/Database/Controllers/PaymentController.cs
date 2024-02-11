using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Database.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        SimbirGoContext _db;
        private HttpRequest _request;
        public PaymentController(SimbirGoContext db, IHttpContextAccessor httpContextAccessor)
        {
            this._db = db;
            _request = httpContextAccessor.HttpContext.Request;
        }
        [Authorize]
        [HttpPost("{accountId}")]
        public IActionResult Hesoyam([FromRoute] int accountId)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x=> x.Username == JwtActions.ReturnUsername(_request));
                if (user.IsAdmin == false)
                {
                    if(user.Id == accountId)
                    {
                        user.Balance += 250000;
                        _db.SaveChanges();
                        return Ok(user);
                    }
                    throw new InvalidOperationException();
                }
                var usr = _db.Users.FirstOrDefault(x=> x.Id == accountId);
                usr.Balance += 25000;
                _db.Update(usr);
                _db.SaveChanges();
                return Ok(usr);
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
    }
}
