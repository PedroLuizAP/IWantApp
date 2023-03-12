using IWantApp.Domain.Products;
using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Mvc;

namespace IWantApp.Endpoints.Categories
{
    public class CategoryPut
    {
        public static string Template => "/categories/{id:guid}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };

        public static Delegate Handle => Action;

        public static IResult Action([FromRoute] Guid id, CategoryRequest  categoryRequest, DataContext context)
        {
            var category = context.Category.Where(c => c.Id == id).FirstOrDefault();

            if (category == null) Results.NotFound();

            category!.Active = categoryRequest.Active;
            category!.Name = categoryRequest.Name;

            context.Category.Update(category);
            context.SaveChanges();

            return Results.Ok();
        }
}
}
