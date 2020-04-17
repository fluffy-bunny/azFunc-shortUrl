using System.Threading.Tasks;

namespace dotnetcore.urlshortener.contracts
{
    public interface IUrlShortenerAlgorithm
    {
        string GenerateUniqueId();
    }
}