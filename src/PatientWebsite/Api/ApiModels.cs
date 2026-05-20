namespace PatientWebsite.Api;

public record RegisterRequest(string Bsn, string FirstName, string LastName);
public record SignInRequest(string Bsn, string LastName);
public record PermissionRequest(bool Approve);
