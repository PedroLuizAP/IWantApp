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
        public static async Task<IResult> Action(LoginRequest loginRequest, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var user = await userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null) return Results.BadRequest();

            if (!await userManager.CheckPasswordAsync(user, loginRequest.Password)) return Results.BadRequest();

            var subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, loginRequest.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            });

            var claims = await userManager.GetClaimsAsync(user);

            subject.AddClaims(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddMinutes(10),
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
