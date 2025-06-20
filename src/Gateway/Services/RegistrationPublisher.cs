using Contracts;
using Gateway.Models;
using MassTransit;

namespace Gateway.Services
{
    public class RegistrationPublisher : IRegistrationPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RegistrationPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public Task PublishAsync(RegisterRequest req)
        {
            var correlationId = Guid.NewGuid();
            var message = new RegistrationRequested(
                correlationId,
                req.Email,
                req.Password,
                req.FirstName,
                req.LastName,
                req.Specialty,
                req.Age
            );


            return _publishEndpoint.Publish(message);

        }
    }
}
