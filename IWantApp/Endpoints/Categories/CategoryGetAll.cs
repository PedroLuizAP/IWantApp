using IWantApp.Domain.Products;
using IWantApp.Infra.Data;

namespace IWantApp.Endpoints.Categories
{
    public class CategoryGetAll
    {
        public static string Template => "/categories";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };

        public static Delegate Handle => Action;

        public static IResult Action(DataContext context)
        {
            var categories = context.Category.ToList();

            var response = categories.Select(c => new CategoryResponse { Id = c.Id, Active = c.Active, Name = c.Name });

            return Results.Ok(response);
        }
    }
}
