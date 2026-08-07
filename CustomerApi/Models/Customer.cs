using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Website { get; set; }

    // Navigation property: EF Core uses this to know a Customer has many Users.
    // Note: this is NOT what we'll return in API responses (spec says hide users
    // from the customer response) — we'll control that with DTOs later.
    public List<User> Users { get; set; } = new();
}