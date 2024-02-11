
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;
using api.Services;
using api.Database.Models;

namespace api.Database.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        private SimbirGoContext _db;
        private HttpRequest _request;
        public AcountController(SimbirGoContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _request = httpContextAccessor.HttpContext.Request;
        }
        [Authorize]
        [HttpGet("Me")]
        public IActionResult Me()
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(_request));
                return Ok(user);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] User user)
        {
            try
            {           
                if (_db.Users.FirstOrDefault(p => p.Username == user.Username) == null)
                {
                    _db.Add(new User
                    {
                        Username = user.Username,
                        Password = user.Password,
                        IsAdmin = false,
                        Balance = 0
                    });
                    _db.SaveChanges();
                    return Ok();
                }
                return BadRequest("This username allready exist");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("SignIn")]
        public IActionResult SignIn([FromBody] User user)
        {
            try
            {
                User? usr = _db.Users.FirstOrDefault(p => p.Username == user.Username && p.Password == user.Password);
                if ( usr is null)
                    return BadRequest("User with this params not found");
                string token = JwtActions.GenerateToken(usr);
                return Ok(token);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }
        [Authorize]
        [HttpPost("SignOut")]
        public IActionResult SignOut()
        {
            try
            {
                JwtActions.AddToBlackList(_request);                
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [Authorize]
        [HttpPut("Update")]
        public IActionResult Update([FromBody] User user)
        {
            try
            {
                var oldUser = _db.Users.FirstOrDefault(x=>x.Username == JwtActions.ReturnUsername(_request));
                if(oldUser != null &&( (_db.Users.FirstOrDefault(x => x.Username == user.Username) is null) || oldUser.Username == user.Username))
                {
                    oldUser.Password = user.Password;
                    oldUser.Username = user.Username;
                    _db.Update(oldUser);
                    _db.SaveChanges();
                    return Ok(JwtActions.GenerateToken(oldUser));
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
