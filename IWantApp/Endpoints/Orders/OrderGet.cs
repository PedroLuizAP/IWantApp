using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IWantApp.Endpoints.Orders
{
    public class OrderGet
    {
        public static string Template => "/orders/{id}";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize]
        public static async Task<IResult> Action(Guid id, HttpContext http, DataContext context, UserManager<IdentityUser> userManager)
        {
            var clientId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var employeeClaim = http.User.Claims.First(c => c.Type == "EmployeeCode").Value;

            var order = context.Orders.Include(o => o.Products).FirstOrDefault(o => o.Id == id);

            if (order?.ClientId != clientId && employeeClaim == null) return Results.Forbid();

            var client = await userManager.FindByIdAsync(order!.ClientId);

            var productResponse = order.Products.Select(p => new OrderProduct(p.Id, p.Name));

            var response = new OrderResponse(order.Id, client!.Email!, order.DeliveryAddress, productResponse);

            return Results.Ok(response);
        }
    }
}
