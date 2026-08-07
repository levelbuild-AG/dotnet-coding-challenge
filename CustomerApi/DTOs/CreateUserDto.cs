using System.ComponentModel.DataAnnotations;

namespace CustomerApi.DTOs;

// What someone SENDS US to create a new user.
// Notice: no CustomerId here - since the URL already tells us which customer
// (e.g. POST /api/customer/5/user), we don't need it repeated in the body.
public class CreateUserDto
{
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
}