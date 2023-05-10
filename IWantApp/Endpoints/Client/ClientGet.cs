﻿using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IWantApp.Endpoints.Client
{
    public class ClientGet
    {
        public static string Template => "/clients";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static async Task<IResult> Action(HttpContext http)
        {
            var user = http.User;

            var result = new
            {
                Id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
                Name = user.Claims.First(c => c.Type == "Name").Value,
                Document = user.Claims.First(c => c.Type == "Document").Value
            };

            return Results.Ok(result);
        }
    }
}