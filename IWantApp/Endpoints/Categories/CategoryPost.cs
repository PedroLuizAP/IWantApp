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
            var category = new Category(categoryRequest.Name, "test", "test");

            if (!category.IsValid)
            {
                var errors = category.Notifications.GroupBy(g => g.Key).ToDictionary(d => d.Key, d=> d.Select(x => x.Message).ToArray());

                return Results.ValidationProblem(errors);
            }
            context.Category.Add(category);
            context.SaveChanges();

            return Results.Created($"/categories/{category.Id}", category.Id) ;
        }
}
}
