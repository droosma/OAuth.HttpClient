using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.HttpClient
{
    public interface Authentication
    {
        Task<AuthenticationHeaderValue> AuthorizationHeader(CancellationToken cancellationToken);
    }
}