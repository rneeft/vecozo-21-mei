using System.ComponentModel.DataAnnotations;

namespace OnlineToestemming.Data;

public sealed class Patient
{
    public required int Id { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public required string Pseudoniem { get; set; }

    public bool? GavePermission { get; set; }
}
