using IdentityModel.Client;
using System.Threading.Tasks;

namespace dotnetcore.oauth2Services
{
    public interface IClientCredentialsManager
    {
        Task<TokenResponse> GetAccessTokenAsync(string tenant);
        Task<TokenResponse> RefreshAccessTokenAsync(string tenant);
    }
}
