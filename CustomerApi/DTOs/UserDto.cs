namespace CustomerApi.DTOs;

// What we SEND BACK when someone asks for a user.
// Notice: only CustomerId (a number), never the full Customer object.
public class UserDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int CustomerId { get; set; }
}