using Gateway.Clients;
using Gateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServiceClient _authClient;

        public AuthController(IAuthServiceClient authClient)
            => _authClient = authClient;

        /// <summary>
        /// Proxy the /auth/validate-token call to AuthService.
        /// </summary>
        [HttpPost("validate-token")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateToken()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var hdr) ||
                string.IsNullOrWhiteSpace(hdr) ||
                !hdr.ToString().StartsWith("Bearer "))
            {
                return BadRequest("Missing or malformed Authorization header.");
            }

            var jwt = hdr.ToString()["Bearer ".Length..].Trim();
            var user = await _authClient.ValidateTokenAsync(jwt);

            if (user is null)
                return Unauthorized();

            return Ok(user);
        }
    }
}
