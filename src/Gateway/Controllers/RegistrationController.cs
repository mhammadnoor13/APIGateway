using Gateway.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IUserRegistrationService _registrationService;

    public RegistrationController(IUserRegistrationService registrationService)
    {
        _registrationService = registrationService;       
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = await _registrationService.RegisterAsync(request);
            return CreatedAtRoute(
                routeName: "GetUser",
                routeValues: new { userId },
                value: null);
        }
        catch (InvalidOperationException ex)
        {

            return StatusCode(502, new { error = ex.Message });
        }
    }
}
