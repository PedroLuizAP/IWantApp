using Dapper;
using IWantApp.Endpoints.Employees;
using Microsoft.Data.SqlClient;

namespace IWantApp.Infra.Data
{
    public class QueryAllUsersWithClaimName
    {
        private readonly IConfiguration _configuration;

        public QueryAllUsersWithClaimName(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<EmployeeResponse> Execute(int? page , int? rows)
        {
            var db = new SqlConnection(_configuration["ConnectionString:IWantDb"]);

            var queryString = "SELECT Email, ClaimValue as Name FROM AspNetUsers u INNER JOIN AspNetUserClaims c ON u.Id = c.UserId AND claimtype = 'Name' ORDER BY name";

            if (page != null && rows != null) queryString += " OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY";

            return db.Query<EmployeeResponse>(queryString, new { page, rows });
        }
    }
}
