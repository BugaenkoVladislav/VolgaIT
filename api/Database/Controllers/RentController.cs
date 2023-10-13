﻿using api.Database.Models;
using api.Database.Views;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api.Database.Controllers
{
    [Route("api")]
    [ApiController]
    public class RentController : ControllerBase
    {
        SimbirGoContext db;
        public RentController(SimbirGoContext db)
        {
            this.db = db;
        }


        [HttpGet("/Rent/Transport")]
        public IActionResult RentTransportRadius(double lat, double @long, double radius)
        {
            try
            {
                List<long> tsId = new List<long>();
                foreach (var t in db.Transports)
                {
                    double d = Math.Sqrt(Math.Pow(Convert.ToDouble(t.Longitude) - @long, 2) + Math.Pow(Convert.ToDouble(t.Latitude) - lat, 2));
                    if (d <= radius)
                        tsId.Add(t.Id);
                }
                var transports = db.TransportInfos.Where(u => tsId.Contains((u.Id.Value))).ToList();
                return Ok(transports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("/Rent/{rentId}")]
        public IActionResult GetRentInfo([FromRoute] int rentId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                RentInfo? rent = db.RentInfos.First(x => x.Id == rentId);
                if (rent == null)
                    return BadRequest("id with this rent not exist");
                if (user.Username == rent.User || user.Username == rent.Owner)
                    return Ok(rent);
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Authorize]
        [HttpGet("/Rent/MyHistory")]
        public IActionResult GetUserRentInfo()//works but i wanna some remade RentInfo and Rents
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                List<RentInfo> rents = db.RentInfos.Where(x => x.User == user.Username).ToList();
                return Ok(rents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("/Rent/TransportHistory/{transportId}")]
        public IActionResult GetUserRentInfo([FromRoute] int transportId)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                Transport? transport = db.Transports.FirstOrDefault(x => x.Id == transportId);
                if (transport is null)
                    return BadRequest("uncorrect Id transport");
                if (transport.IdOwner == user.Id)
                {
                    List<RentInfo> rentInfos = db.RentInfos.Where(x => x.IdTransport == transportId).ToList();
                    return Ok(rentInfos);
                }
                return BadRequest("not owner");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("/Rent/New/{transportId}")]
        public IActionResult RentNew([FromRoute] long transportId, string rentType)//подчисти мусор
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                RentType priceType = db.RentTypes.First(x => x.RentType1 == rentType);
                double priceOfUnit;
                switch (priceType.Id)
                {
                    case 1:
                        priceOfUnit = Convert.ToDouble(db.Transports.First(x => x.Id == transportId).MinutePrice);
                        break;
                    case 2:
                        priceOfUnit = Convert.ToDouble(db.Transports.First(x => x.Id == transportId).DayPrice);
                        break;
                    default:
                        return BadRequest("not have this type");
                }

                db.Add(new Rent
                {
                    TimeStart = DateTime.UtcNow,
                    IdUser = user.Id,
                    IdTransport = transportId,
                    PriceType = priceType.Id,
                    PriceOfUnit = priceOfUnit
                });
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("/Rent/End/{rentId}")]
        public IActionResult RentEnd([FromRoute] long rentId, double lat, double @long)
        {
            try
            {
                var jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                User? user = db.Users.First(x => x.Username == JwtActions.ReturnUsername(jwt));
                Rent rent = db.Rents.First(x => x.Id == rentId);
                Transport transport = db.Transports.First(x=>x.Id == rent.IdTransport);
                if (rent.IdUser == user.Id)
                {
                    rent.TimeEnd = DateTime.UtcNow;
                    TimeSpan difference = DateTime.UtcNow - rent.TimeStart;
                    if(rent.PriceType == 2)
                    {
                        rent.FinalPrice = difference.Days + 1 * rent.PriceOfUnit;
                    }                       
                    else if(rent.PriceType == 1)
                    {
                        rent.FinalPrice = (difference.Minutes + (difference.Hours * 60) + (difference.Days * 24 * 60)) * rent.PriceOfUnit;                       
                    }
                    db.Update(rent);                   
                    transport.Latitude = lat;
                    transport.Longitude = @long;
                    db.Update(transport);
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest("not your rent");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}