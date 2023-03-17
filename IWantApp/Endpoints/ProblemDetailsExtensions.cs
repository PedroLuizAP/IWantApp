using Flunt.Notifications;
using System.Runtime.CompilerServices;

namespace IWantApp.Endpoints
{
    public static class ProblemDetailsExtensions
    {
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> notification) => notification.GroupBy(g => g.Key).ToDictionary(d => d.Key, d=> d.Select(x => x.Message).ToArray());
    }
}
