using Dapper;
using Microsoft.Data.SqlClient;

namespace IWantApp.Endpoints.Employees
{
    public class EmployeeGetAll
    {
        public static string Template => "/employees";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };

        public static Delegate Handle => Action;

        public static IResult Action(int? page, int? rows, IConfiguration configuration)
        {
            if (page > 10) return Results.BadRequest("Maximum limit of 10 rows.");

            var db = new SqlConnection(configuration["ConnectionString:IWantDb"]);

            var queryString = "SELECT Email, ClaimValue as Name FROM AspNetUsers u INNER JOIN AspNetUserClaims c ON u.Id = c.UserId AND claimtype = 'Name' ORDER BY name";

            if (page != null && rows != null) queryString += " OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY";

            var employees = db.Query<EmployeeResponse>(queryString, new { page, rows });

            return Results.Ok(employees);
        }
    }
}
