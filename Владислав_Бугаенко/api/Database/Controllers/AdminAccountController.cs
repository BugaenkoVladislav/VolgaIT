﻿using api.Database.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Thinktecture.IdentityModel.Authorization.Mvc;

namespace api.Database.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/")]
    [ApiController]
    public class AdminAccountController : ControllerBase
    {
        SimbirGoContext db;

        public AdminAccountController(SimbirGoContext db)
        {
            this.db = db;
                    
        }
        [HttpGet("")]
        public IActionResult GetAllUsers(int start, int count)
        {
            try
            {
                var users = db.Users.Skip(start).Take(count).ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("{id}")]
        public IActionResult GetUser([FromRoute] int id)
        {
            try
            {
                var user = db.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                    return NotFound("Id not exist");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("")]
        public IActionResult CreateNewUser([FromBody] User user)
        {
            try
            {
                if (db.Users.FirstOrDefault(x=> x.Username == user.Username) == null )
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

        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute]int  id, [FromBody] User user)
        {
            try
            {
                var existUser = db.Users.FirstOrDefault(x => x.Id == id);
                if (existUser == null)
                    return NotFound("User with this id not exist");                
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
        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] int id)
        {
            try
            {
                var user = db.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                    return NotFound("Id not exist");
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
