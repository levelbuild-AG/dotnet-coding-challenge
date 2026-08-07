using System.ComponentModel.DataAnnotations;

namespace CustomerApi.DTOs;

// What someone SENDS US to create a new customer
public class CreateCustomerDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Website { get; set; }
}