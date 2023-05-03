using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Endpoints.Products
{
    public class ProductGetShowcase
    {
        public static string Template = "/products/showcase";

        public static string[] Methods = new string[] { HttpMethod.Get.ToString() };

        public static Delegate Handle = Action;
        [AllowAnonymous]
        public static async Task<IResult> Action(DataContext context, int page = 1, int row = 10, string orderBy = "name")
        {
            if (row > 10) return Results.Problem(statusCode: 400, title: "The maximum number of rows is 10.");

            var queryBase = context.Products.AsNoTracking().Include(p => p.Category).Where(p => p.HasStock && p.Category.Active);

            var queryFilter = queryBase.Skip((page - 1) * row).Take(row);

            switch (orderBy.ToUpper())
            {
                case "NAME":
                    queryFilter = queryFilter.OrderBy(p => p.Name);
                    break;

                case "PRICE":
                    queryFilter = queryFilter.OrderBy(p => p.Price);
                    break;

                default:
                    return Results.Problem(statusCode: 400, title: "Invalid order by.");
            }

            var products = queryFilter.ToList();

            var results = products.Select(p => new ProductResponse(p.Name, p.Category, p.Description, p.HasStock, p.Price, p.Active));

            return Results.Ok(results);
        }
    }
}
