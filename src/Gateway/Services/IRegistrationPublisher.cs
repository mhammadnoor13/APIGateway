using Gateway.Models;

namespace Gateway.Services;

public interface IRegistrationPublisher
{
    Task PublishAsync(RegisterRequest request);
}