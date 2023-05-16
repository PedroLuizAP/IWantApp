using IWantApp.Infra.Data;
using IWantApp.Resources;
using Microsoft.AspNetCore.Authorization;

namespace IWantApp.Endpoints.Employees
{
    public class EmployeeGetAll
    {
        public static string Template => "/employees";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };

        public static Delegate Handle => Action;
        [Authorize(Policy = "Employee005Policy")]
        public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimName query)
        {
            if (page > 10) return Results.BadRequest(Messages.LimitRows);

            return Results.Ok(await query.Execute(page, rows));
        }
    }
}
