using api.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;

namespace api.Database.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        SimbirGoContext db;
        public AcountController(SimbirGoContext db)
        {
            this.db = db;
        }

        [Authorize]
        [HttpDelete("/ChnageUser")]
        public IActionResult SignIn(string username)
        {
            try
            {
                User? user = db.Users.FirstOrDefault(x => x.Username == username);
                if (user == null) 
                {
                    return BadRequest();
                }
                db.Remove(user);
                db.SaveChanges();
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("/SignIn")]
        public IActionResult Token(string username, string password)
        {
            try
            {
                // находим пользователя 
                User? user = db.Users.FirstOrDefault(p => p.Username == username && p.Password == password);
                // если пользователь не найден, отправляем статусный код 401
                if (user is null)
                    return StatusCode(401);

                var role = user.IsAdmin ? "Admin" : "User";
                var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),};
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                // формируем ответ
                var response = new
                {
                    access_token = encodedJwt,
                    username = user.Username
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }

 

    }
}
