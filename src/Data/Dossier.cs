using System.ComponentModel.DataAnnotations;

namespace OnlineToestemming.Data;

public sealed class Dossier
{
    public required int Id { get; set; }

    [Required]
    public required string healthcareCompanyId { get; set; }

    [Required]
    public required string Pseudoniem { get; set; }

    [Required]
    public required HealthcareCompany healthcareCompany { get; init; }

}
