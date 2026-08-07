using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    // Foreign key: every User belongs to exactly one Customer.
    [Required]
    public int CustomerId { get; set; }

    // Navigation property back to the parent Customer.
    public Customer? Customer { get; set; }
}