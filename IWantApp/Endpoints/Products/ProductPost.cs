using IWantApp.Domain.Products;
using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IWantApp.Endpoints.Products
{
    public class ProductPost
    {
        public static string Template => "/products";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "Employeeolicy")]
        public static async Task<IResult> Action(ProductRequest productRequest, HttpContext http, DataContext context)
        {
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var category = await context.Category.FirstOrDefaultAsync(c => c.Id == productRequest.CategoryId);
        
            var product = new Product(productRequest.Name, category, productRequest.Description, productRequest.HasStock, userId);
        
            if(!product.IsValid) return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());

            await context.Products.AddAsync(product);

            await context.SaveChangesAsync();

            return Results.Created($"/products/{product.Id}", product);
        }
    }
}
