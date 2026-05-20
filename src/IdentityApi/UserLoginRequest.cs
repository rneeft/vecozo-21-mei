namespace OnlineToestemming.IdentityApi;

public class UserLoginRequest
{
    public required string Bsn { get; init; }
    public required string LastName { get; init; }
}
