using Dapper;
using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

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
            if (page > 10) return Results.BadRequest("Maximum limit of 10 rows.");

            return Results.Ok(await query.Execute(page, rows));
        }
    }
}
