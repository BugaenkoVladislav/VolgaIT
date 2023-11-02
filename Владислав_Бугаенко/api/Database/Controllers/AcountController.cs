
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
    [Route("api")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        SimbirGoContext db;
        public AcountController(SimbirGoContext db)
        {
            this.db = db;
        }

        [Authorize]
        [HttpGet("/api/Account/Me")]
        public IActionResult Me()
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");//получаем наш jwt токен
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null)
                    return NotFound("Uncorrect Token");
                return Ok(user);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/api/Account/SignUp")]
        public IActionResult SignUp([FromBody] User user)
        {
            try
            {           
                if (db.Users.FirstOrDefault(p => p.Username == user.Username) == null)
                {
                    db.Add(new User
                    {
                        Username = user.Username,
                        Password = user.Password,
                        IsAdmin = false,
                        Balance = 0
                    });
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest("This username allready exist");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }





        [HttpPost("/api/Account/SignIn")]
        public IActionResult SignIn([FromBody] User user)
        {
            try
            {
                User? usr = db.Users.FirstOrDefault(p => p.Username == user.Username && p.Password == user.Password);
                // находим пользователя 
                if ( usr is null)
                    return BadRequest("User with this params not found");
                string token = JwtActions.GenerateToken(usr);
                return Ok(token);//возвращаем токен
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }
        [Authorize]
        [HttpPost("/api/Account/SignOut")]
        public IActionResult SignOut()
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");//получаем наш jwt токен
                JwtActions.AddToBlackList(jwt);                
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }



        [Authorize]
        [HttpPut("/api/Account/Update")]
        public IActionResult Update([FromBody] User user)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ","");
                User? jwtUsr = db.Users.FirstOrDefault(x=>x.Username == JwtActions.ReturnUsername(jwt));
                if (jwtUsr is null)
                    return NotFound("Uncorrect Token");
                if((db.Users.FirstOrDefault(x => x.Username == user.Username) is null) || jwtUsr.Username == user.Username)
                {
                    jwtUsr.Password = user.Password;
                    jwtUsr.Username = user.Username;
                    db.Update(jwtUsr);
                    db.SaveChanges();
                    return Ok(JwtActions.GenerateToken(jwtUsr));
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
