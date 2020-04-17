using dotnetcore.urlshortener.contracts.Models;
using IdentityModel.Client;
using System.Threading.Tasks;

namespace dotnetcore.urlshortener.contracts
{
    public interface ITenantServices
    {
        Task<TokenResponse> GetAccessTokenAsync();
        Task<TokenResponse> RefreshAccessTokenAsync();

    }
    public interface ITenantStore
    {
        Task<ITenantServices> GetTenantAsync(string tenant);

    }
}
