using System.Threading.Tasks;

namespace CleanArchitecture.Common.Security
{
    public interface ISecurityServiceAgent
    {
        Task<string> GetPasswordTokenAsync();

        Task<string> GetClientCredentialsTokenAsync();
        
        Task<string> GetDelegationTokenAsync(string userToken);
        
        Task<string> GetUserProfileIdAsync();

        Task<string> GetUserProfileIdAsync(string accessToken);
    }
}
