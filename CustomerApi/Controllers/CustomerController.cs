using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using CustomerApi.Data;
using CustomerApi.DTOs;
using CustomerApi.Models;


namespace CustomerApi.Controllers;

[ApiController]
[Route("api/customer")]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _context;

    // ASP.NET Core automatically hands us a working AppDbContext here.
    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/customer
    // Supports $select, $filter, $top, $skip, $orderby via [EnableQuery]
    [HttpGet]
    [EnableQuery]
    public IQueryable<CustomerDto> GetCustomers()
    {
        return _context.Customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Website = c.Website
        });
    }

    // GET /api/customer/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound(); // 404
        }

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Website = customer.Website
        };
    }

    // POST /api/customer
   // POST /api/customer
[HttpPost]
public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto input)
{
    var nameExists = await _context.Customers.AnyAsync(c => c.Name == input.Name);
    if (nameExists)
    {
        return Conflict(new { message = $"A customer named '{input.Name}' already exists." });
    }

    var customer = new Customer
    {
        Name = input.Name,
        Website = input.Website
    };

    _context.Customers.Add(customer);
    await _context.SaveChangesAsync();

    var result = new CustomerDto
    {
        Id = customer.Id,
        Name = customer.Name,
        Website = customer.Website
    };

    return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, result);
}
   // PATCH /api/customer/5
[HttpPatch("{id}")]
public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, UpdateCustomerDto input)
{
    var customer = await _context.Customers.FindAsync(id);

    if (customer == null)
    {
        return NotFound();
    }

    if (input.Name != null)
    {
        var nameTaken = await _context.Customers.AnyAsync(c => c.Name == input.Name && c.Id != id);
        if (nameTaken)
        {
            return Conflict(new { message = $"A customer named '{input.Name}' already exists." });
        }
        customer.Name = input.Name;
    }

    if (input.Website != null)
    {
        customer.Website = input.Website;
    }

    await _context.SaveChangesAsync();

    return new CustomerDto
    {
        Id = customer.Id,
        Name = customer.Name,
        Website = customer.Website
    };
}

    // DELETE /api/customer/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound(); // 404
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(); // Users deleted automatically (cascade)

        return NoContent(); // 204
    }
}