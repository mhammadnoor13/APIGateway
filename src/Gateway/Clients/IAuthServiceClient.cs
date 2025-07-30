using Gateway.Models;

namespace Gateway.Clients
{
    public interface IAuthServiceClient
    {
        Task<ValidateTokenResponse?> ValidateTokenAsync(string jwt);

    }
}
