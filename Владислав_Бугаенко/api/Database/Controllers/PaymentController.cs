using api.Database.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Database.Controllers
{
    [Route("api")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        SimbirGoContext db;
        public PaymentController(SimbirGoContext db)
        {
            this.db = db;
        }
        [Authorize]
        [HttpPost("/api/Payment/Hesoyam/{accountId}")]
        public IActionResult Hesoyam([FromRoute] int accountId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ","");
                User? user = db.Users.FirstOrDefault(x=> x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect token");
                if (user.IsAdmin == false)
                {
                    if(user.Id == accountId)
                    {
                        user.Balance += 250000;
                        db.SaveChanges();
                        return Ok(user);
                    }
                    return BadRequest("you cant add ballance for other users");
                }
                User? usr = db.Users.FirstOrDefault(x=> x.Id == accountId);
                if (usr == null)
                    return NotFound("Uncorrect token");
                usr.Balance += 250000;
                db.SaveChanges();
                return Ok(usr);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
