using Gateway.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationPublisher _registrationPublisher;

    public RegistrationController(IRegistrationPublisher registrationPublisher)
    {
        _registrationPublisher = registrationPublisher;       
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        await _registrationPublisher.PublishAsync(request);
        return Ok("Pubished");
    }
}
