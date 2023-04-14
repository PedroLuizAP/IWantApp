using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IWantApp.Endpoints.Security
{
    public class TokenPost
    {
        public static string Template => "/token";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };

        public static Delegate Handle => Action;
        [AllowAnonymous]
        public static IResult Action(LoginRequest loginRequest, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var user = userManager.FindByEmailAsync(loginRequest.Email).Result;

            if (user == null) return Results.BadRequest();

            if (!userManager.CheckPasswordAsync(user, loginRequest.Password).Result) return Results.BadRequest();

            var subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, loginRequest.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            });

            var claims = userManager.GetClaimsAsync(user).Result;

            subject.AddClaims(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = configuration["JwtBearerTokenSettings:Issuer"],
                Audience = configuration["JwtBearerTokenSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]!)), SecurityAlgorithms.HmacSha256Signature)//simulate key
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Results.Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
