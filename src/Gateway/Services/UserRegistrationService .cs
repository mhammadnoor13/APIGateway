using Contracts.Contracts;
using Contracts.Responses;
using Gateway.Models;
using MassTransit;

namespace Gateway.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IRequestClient<CreateUserCommand> _createUserClient;
        private readonly IRequestClient<CreateConsultantProfileCommand> _createProfileClient;

        public UserRegistrationService(
            IRequestClient<CreateUserCommand> createUserClient,
            IRequestClient<CreateConsultantProfileCommand> createProfileClient)
        {
            _createUserClient = createUserClient;
            _createProfileClient = createProfileClient;
        }

        public async Task<Guid> RegisterAsync(RegisterRequest req)
        {
            var usr_cmd = new CreateUserCommand(
                Guid.NewGuid(),           
                req.Email,
                req.Password
            );

            Guid userId;
            try
            {
                var response = await _createUserClient
                    .GetResponse<UserCreatedResponse>(usr_cmd);

                userId = response.Message.UserId;
            }
            catch(RequestFaultException ex)
            {
                throw new InvalidOperationException("AuthService failed to create user.", ex);

            }


            var cons_cmd = new CreateConsultantProfileCommand(
                    userId,
                    req.FirstName,
                    req.LastName,
                    req.Specialty,
                    req.Age);

            try
            {

                var profileResponse = await _createProfileClient
                    .GetResponse<ConsultantProfileCreatedResponse>(cons_cmd);
                return profileResponse.Message.UserId;

            }
            catch(RequestFaultException ex)
            {

                throw new InvalidOperationException("ConsultantService failed to create profile.",ex);
            }



        

        }
    }
}
