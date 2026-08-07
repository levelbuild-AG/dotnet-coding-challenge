using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using CustomerApi.Data;
using CustomerApi.DTOs;
using CustomerApi.Models;

namespace CustomerApi.Controllers;

// Notice the route includes {customerId} - every action here happens
// "inside" a specific customer.
[ApiController]
[Route("api/customer/{customerId}/user")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // Small helper: checks the customer exists before we do anything else.
    // Used by every method below, so we don't repeat this check five times.
    private async Task<bool> CustomerExists(int customerId)
    {
        return await _context.Customers.AnyAsync(c => c.Id == customerId);
    }

    // GET /api/customer/5/user
    [HttpGet]
    [EnableQuery]
    public async Task<ActionResult<IQueryable<UserDto>>> GetUsers(int customerId)
    {
        if (!await CustomerExists(customerId))
        {
            return NotFound(new { message = $"No customer found with id {customerId}." });
        }

        var users = _context.Users
            .Where(u => u.CustomerId == customerId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DateOfBirth = u.DateOfBirth,
                CustomerId = u.CustomerId
            });

        return Ok(users);
    }

    // GET /api/customer/5/user/2
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDto>> GetUser(int customerId, int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.CustomerId == customerId);

        if (user == null)
        {
            return NotFound(new { message = $"No user found with id {userId} for customer {customerId}." });
        }

        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            CustomerId = user.CustomerId
        };
    }

    // POST /api/customer/5/user
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(int customerId, CreateUserDto input)
    {
        if (!await CustomerExists(customerId))
        {
            return NotFound(new { message = $"No customer found with id {customerId}." });
        }

        var displayNameTaken = await _context.Users.AnyAsync(u => u.DisplayName == input.DisplayName);
        if (displayNameTaken)
        {
            return Conflict(new { message = "A user with this display name already exists. Please choose a different display name." });
        }

        var emailTaken = await _context.Users.AnyAsync(u => u.Email == input.Email);
        if (emailTaken)
        {
            return Conflict(new { message = "A user with this email address already exists. Please use a different email." });
        }

        var user = new User
        {
            DisplayName = input.DisplayName,
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            DateOfBirth = input.DateOfBirth,
            CustomerId = customerId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            CustomerId = user.CustomerId
        };

        return CreatedAtAction(nameof(GetUser), new { customerId, userId = user.Id }, result);
    }

    // PATCH /api/customer/5/user/2
    [HttpPatch("{userId}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int customerId, int userId, UpdateUserDto input)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.CustomerId == customerId);

        if (user == null)
        {
            return NotFound(new { message = $"No user found with id {userId} for customer {customerId}." });
        }

        if (input.DisplayName != null)
        {
            var taken = await _context.Users.AnyAsync(u => u.DisplayName == input.DisplayName && u.Id != userId);
            if (taken)
            {
                return Conflict(new { message = "A user with this display name already exists. Please choose a different display name." });
            }
            user.DisplayName = input.DisplayName;
        }

        if (input.Email != null)
        {
            var taken = await _context.Users.AnyAsync(u => u.Email == input.Email && u.Id != userId);
            if (taken)
            {
                return Conflict(new { message = "A user with this email address already exists. Please use a different email." });
            }
            user.Email = input.Email;
        }

        if (input.FirstName != null)
        {
            user.FirstName = input.FirstName;
        }

        if (input.LastName != null)
        {
            user.LastName = input.LastName;
        }

        if (input.DateOfBirth != null)
        {
            user.DateOfBirth = input.DateOfBirth.Value;
        }

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            CustomerId = user.CustomerId
        };
    }

    // DELETE /api/customer/5/user/2
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int customerId, int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.CustomerId == customerId);

        if (user == null)
        {
            return NotFound(new { message = $"No user found with id {userId} for customer {customerId}." });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}