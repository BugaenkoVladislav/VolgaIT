using api.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static Thinktecture.IdentityModel.Constants.JwtConstants;

namespace api.Services
{
    public class JwtActions
    {
        static List<string> bannedTokens = new List<string>();
        public static  void AddToBlackList(HttpRequest request)
        {
            bannedTokens.Add(request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
        }

        public static string ReturnUsername(HttpRequest request)
        {
            var jwt = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (bannedTokens.Contains(jwt))
            {
                return String.Empty;
            }                
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwt);//считываем токен
            var user = token.Claims.First(c => c.Type == ClaimTypes.Name).Value; //достаем из токена claim
            return user;
        }
        public static string GenerateToken(User user)
        {
            var role = user.IsAdmin ? "Admin" : "User";
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role)};
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return "Bearer "+ encodedJwt;//возвращаем токен
        }
        
        public static List<Transport> FindInRadius(List<Transport> list,double lat, double @long, double radius)
        {
            var result = new List<Transport>();
            foreach (var i in list)
            {
                double d = Math.Sqrt(Math.Pow(Convert.ToDouble(i.Longitude) - @long, 2) + Math.Pow(Convert.ToDouble(i.Latitude) - lat, 2));
                if (d <= radius)
                    result.Add(i);
            }
            return result;
        }

    }
}
