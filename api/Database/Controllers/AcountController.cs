using api.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;
using api.Services;

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
        [HttpGet("/Account/Me")]
        public IActionResult Me()
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");//получаем наш jwt токен
                User? user = db.Users.FirstOrDefault(x => x.Username == JwtActions.ReturnUsername(jwt));
                if (user == null) 
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/Account/SignUp")]
        public IActionResult SignUp([FromBody] User user)
        {
            try
            {
                User? usr = db.Users.FirstOrDefault(p => p.Username == user.Username);                
                if (usr is null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return Ok();
                }
                return StatusCode(401);


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }





        [HttpPost("/Account/SignIn")]
        public IActionResult SignIn([FromBody] User user)
        {
            try
            {
                User? usr = db.Users.FirstOrDefault(p => p.Username == user.Username && p.Password == user.Password);
                // находим пользователя 
                if ( usr is null)
                    return StatusCode(401);
                string token = JwtActions.GenerateToken(usr);
                return Ok(token);//возвращаем токен
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }
        [Authorize]
        [HttpPost("/Account/SignOut")]
        public IActionResult SignOut()
        {
            try
            {  
                //реализация отзыва токенов cоздай блэклист лог в бд или в софте
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }



        [Authorize]
        [HttpPut("/Account/Update")]
        public IActionResult Update([FromBody] User user)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ","");
                User? jwtUsr = db.Users.First(x=>x.Username == JwtActions.ReturnUsername(jwt));
                if (jwtUsr is null)
                    return StatusCode(401);
                if((db.Users.FirstOrDefault(x => x.Username == user.Username) is null) || jwtUsr.Username == user.Username)
                {
                    jwtUsr.Password = user.Password;
                    jwtUsr.Username = user.Username;
                    db.Users.Update(jwtUsr);
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
