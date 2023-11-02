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
        public static  void AddToBlackList(string jwt)
        {
            bannedTokens.Add(jwt);
        }

        public static string ReturnUsername(string jwt)
        {
            JwtActions actions = new JwtActions();
            if (bannedTokens.Contains(jwt))
            {
                return "";
            }                
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwt);//считываем токен
            var claim = token.Claims.First(c => c.Type == ClaimTypes.Name).Value; //достаем из токена claim
            return claim;
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
            return "Bearer "+encodedJwt;//возвращаем токен
        }

    }
}
