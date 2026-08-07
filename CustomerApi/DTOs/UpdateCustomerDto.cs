using System.ComponentModel.DataAnnotations;

namespace CustomerApi.DTOs;

// What someone SENDS US to update an existing customer.
// Fields are optional (nullable) because PATCH means "only change what I send".
public class UpdateCustomerDto
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? Website { get; set; }
}