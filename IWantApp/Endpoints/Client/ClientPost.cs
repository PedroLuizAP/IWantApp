using IWantApp.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IWantApp.Endpoints.Client
{
    public class ClientPost
    {
        public static string Template => "/clients";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static async Task<IResult> Action(ClientRequest clientRequest, UserCreator userCreator)
        {
            var userClaims = new List<Claim>
            {
                new Claim("Document", clientRequest.Document),
                new Claim("Name", clientRequest.Name),
            };

            (var result, var userId) = await userCreator.Create(clientRequest.Email, clientRequest.Password, userClaims);

            if (!result.Succeeded) return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

            return Results.Created($"/clients/{userId}", userId);
        }
    }
}
