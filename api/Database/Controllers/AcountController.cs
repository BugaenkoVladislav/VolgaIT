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
    [Route("api/Account")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        SimbirGoContext db;
        public AcountController(SimbirGoContext db)
        {
            this.db = db;
        }

        [Authorize]
        [HttpGet("/Me")]
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

        [HttpPost("/SignUp")]
        public IActionResult SignUp(string username, string password)
        {
            try
            {
                // находим пользователя 
                User? user = db.Users.FirstOrDefault(p => p.Username == username );
                // если пользователь не найден, отправляем статусный код 401
                if (user is null)
                {
                    db.Add(new User
                    {
                        Username = username,
                        Password = password,
                        IsAdmin = false
                    });
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





        [HttpPost("/SignIn")]
        public IActionResult SignIn(string username, string password)
        {
            try
            {
                // находим пользователя 
                User? user = db.Users.FirstOrDefault(p => p.Username == username && p.Password == password);
                if (user is null)
                    return StatusCode(401);
                string token = JwtActions.GenerateToken(user);
                return Ok(token);//возвращаем токен
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }
        [Authorize]
        [HttpPost("/SignOut")]
        public IActionResult SignOut()
        {
            try
            {  
                //реализация отзыва токенов
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpPut("/Update")]
        public IActionResult Update(string newUsername, string newPassword)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ","");
                User? user = db.Users.First(x=>x.Username == JwtActions.ReturnUsername(jwt));
                if (user is null)
                    return BadRequest();
                if(db.Users.FirstOrDefault(user=>user.Username == newUsername) is null)
                {
                    user.Password = newPassword;
                    user.Username = newUsername;
                    db.Update(user);
                    db.SaveChanges();
                    string token = JwtActions.GenerateToken(user);
                    return Ok(token);
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        


    }
}
