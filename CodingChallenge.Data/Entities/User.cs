namespace Levelbuild.CodingChallenge.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public Customer Customer { get; set; }
    
    public string DisplayName { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public DateTime DateOfBirth { get; set; }
}