using System.ComponentModel.DataAnnotations;

namespace OnlineToestemming.Data;

public sealed class HealthcareCompany
{
    public required int Id { get; init; }

    [Required]
    public required string CompanyId { get; init; }

    [Required]
    public required string CompanyName { get; set; }
}
