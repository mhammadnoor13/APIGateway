using Gateway.Models;

namespace Gateway.Services;

public interface IUserRegistrationService
{
    Task<Guid> RegisterAsync(RegisterRequest request);
}