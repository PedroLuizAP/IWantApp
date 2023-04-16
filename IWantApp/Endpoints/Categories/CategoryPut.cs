using IWantApp.Domain.Products;
using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IWantApp.Endpoints.Categories
{
    public class CategoryPut
    {
        public static string Template => "/categories/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmplyeePolicy")]
        public static IResult Action([FromRoute] Guid id, CategoryRequest categoryRequest, HttpContext http, DataContext context)
        {
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var category = context.Category.Where(c => c.Id == id).FirstOrDefault();

            if (category == null) Results.NotFound();

            category!.EditInfo(categoryRequest.Name, categoryRequest.Active, userId);

            if (!category.IsValid) return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());

            context.Category.Update(category);

            context.SaveChanges();

            return Results.Ok();
        }
    }
}
