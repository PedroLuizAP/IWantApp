using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace IWantApp.Endpoints
{
    public static class ProblemDetailsExtensions
    {
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notification) => notification.GroupBy(g => g.Key).ToDictionary(d => d.Key, d => d.Select(x => x.Message).ToArray());
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> errors)
        {
            var dictionary = new Dictionary<string, string[]>();
            dictionary.Add("Error", errors.Select(e => e.Description).ToArray());
            return errors.GroupBy(e => e.Code).ToDictionary(d => d.Key, d => d.Select(x => x.Description).ToArray());
        }
    }
}
