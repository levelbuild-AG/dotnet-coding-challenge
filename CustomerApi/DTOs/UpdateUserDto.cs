using System.ComponentModel.DataAnnotations;

namespace CustomerApi.DTOs;

public class UpdateUserDto
{
    [MaxLength(255)]
    public string? DisplayName { get; set; }

    [MaxLength(255)]
    public string? FirstName { get; set; }

    [MaxLength(255)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }
}