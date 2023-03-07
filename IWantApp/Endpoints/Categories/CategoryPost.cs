using IWantApp.Domain.Products;
using IWantApp.Infra.Data;

namespace IWantApp.Endpoints.Categories
{
    public class CategoryPost
    {
        public static string Template => "/categories";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };

        public static Delegate Handle => Action;

        public static IResult Action(CategoryRequest  categoryRequest, DataContext context)
        {
            var category = new Category(categoryRequest.Name)
            {
                CreatedBy = "Debug",
                EditedBy = "",
                EditedOn = DateTime.Now,
                CreatedOn = DateTime.Now,
            };

            if (!category.IsValid) return Results.BadRequest(category.Notifications);

            context.Category.Add(category);
            context.SaveChanges();

            return Results.Created($"/categories/{category.Id}", category.Id) ;
        }
}
}
