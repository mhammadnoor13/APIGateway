namespace Gateway.Models
{
    public record ValidateTokenResponse(
        Guid UserId,
        string Email,
        string[] Roles
    );
}
