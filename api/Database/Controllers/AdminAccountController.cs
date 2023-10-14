using api.Database.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Thinktecture.IdentityModel.Authorization.Mvc;

namespace api.Database.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api")]
    [ApiController]
    public class AdminAccountController : ControllerBase
    {
        SimbirGoContext db;
        public AdminAccountController(SimbirGoContext db)
        {
            this.db = db;
        }
        [HttpGet("/Admin/Account")]
        public IActionResult GetAllUsers(int start, int count)
        {
            try
            {               
                var users = db.Users.Where(x => x.Id >= start && x.Id <= start + count).ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("/Admin/Account/{id}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            try
            {
                User? user = db.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                    return BadRequest("Id not exist");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/Admin/Account")]
        public IActionResult CreateNewUser([FromBody] User user)
        {
            try
            {
                if(db.Users.FirstOrDefault(x=> x.Username == user.Username) == null )
                {
                    db.Add(user);
                    db.SaveChanges();
                    return Ok(user);
                }
                return BadRequest("This username already exist");
                
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("/Admin/Account/{id}")]
        public IActionResult UpdateUser([FromRoute]int  id, [FromBody] User user)
        {
            try
            {
                User? existUser = db.Users.FirstOrDefault(x => x.Id == id);
                if (existUser == null)
                    return BadRequest("User with this id not exist");                
                if (db.Users.First(x => x.Username == user.Username) == null || existUser.Username == user.Username)
                {
                    existUser.Username = user.Username;
                    existUser.Password   = user.Password;
                    existUser.IsAdmin = user.IsAdmin;
                    existUser.Balance = user.Balance;
                    db.Update(existUser);
                    db.SaveChanges();
                    return Ok(existUser);
                }
                return BadRequest("This username already exist");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("/Admin/Account/{id}")]
        public IActionResult DeleteUser([FromRoute] int id)
        {
            try
            {
                User? user = db.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                    return BadRequest("Id not exist");
                db.Remove(user);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
