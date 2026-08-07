namespace CustomerApi.DTOs;

// What we SEND BACK to whoever calls the API
public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
}