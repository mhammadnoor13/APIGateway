namespace Gateway.Models
{
    public record RegisterRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string Specialty,
        int Age
    );
}
