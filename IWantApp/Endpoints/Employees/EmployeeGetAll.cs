using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace IWantApp.Endpoints.Employees
{
    public class EmployeeGetAll
    {
        public static string Template => "/employees";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };

        public static Delegate Handle => Action;

        public static IResult Action(int page, int rows, IConfiguration configuration)
        {
            var db = new SqlConnection(configuration["ConnectionString:IWantDb"]);

            //var employees = db.Query<>()

            return Results.Ok();
        }
    }
}
