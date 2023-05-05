﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IWantApp.Endpoints.Client
{
    public class ClientPost
    {
        public static string Template => "/clients";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static async Task<IResult> Action(ClientRequest clientRequest, HttpContext http, UserManager<IdentityUser> userManager)
        {
            var newUser = new IdentityUser { UserName = clientRequest.Email, Email = clientRequest.Email };

            var result = await userManager.CreateAsync(newUser, clientRequest.Password);

            if (!result.Succeeded) return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());

            var userClaims = new List<Claim>
            {
                new Claim("Document", clientRequest.Document),
                new Claim("Name", clientRequest.Name),
            };

            var claimResult = await userManager.AddClaimsAsync(newUser, userClaims);

            if (!claimResult.Succeeded) return Results.BadRequest(claimResult.Errors.First());

            return Results.Created($"/employees/{newUser.Id}", newUser.Id);
        }
    }
}
