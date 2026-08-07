using Microsoft.EntityFrameworkCore;
using CustomerApi.Controllers;
using CustomerApi.Data;
using CustomerApi.DTOs;
using CustomerApi.Models;
using Xunit;

namespace CustomerApi.Tests;

public class UserControllerTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateUser_ValidInput_ReturnsCreatedUser()
    {
        // Arrange: need a customer to attach the user to first
        var context = CreateInMemoryContext();
        var customer = new Customer { Name = "Acme", Website = null };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var controller = new UserController(context);
        var input = new CreateUserDto
        {
            DisplayName = "jane.doe",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@acme.com",
            DateOfBirth = new DateTime(1990, 5, 1)
        };

        // Act
        var result = await controller.CreateUser(customer.Id, input);

        // Assert
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(result.Result);
        var user = Assert.IsType<UserDto>(createdResult.Value);
        Assert.Equal("jane.doe", user.DisplayName);
        Assert.Equal(customer.Id, user.CustomerId);
    }

    [Fact]
    public async Task CreateUser_NonExistentCustomer_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var controller = new UserController(context);
        var input = new CreateUserDto
        {
            DisplayName = "jane.doe",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@acme.com",
            DateOfBirth = new DateTime(1990, 5, 1)
        };

        // Act: customer id 999 doesn't exist
        var result = await controller.CreateUser(999, input);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var customer = new Customer { Name = "Acme", Website = null };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        context.Users.Add(new User
        {
            DisplayName = "existing.user",
            FirstName = "Existing",
            LastName = "User",
            Email = "duplicate@acme.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            CustomerId = customer.Id
        });
        await context.SaveChangesAsync();

        var controller = new UserController(context);
        var input = new CreateUserDto
        {
            DisplayName = "new.user",
            FirstName = "New",
            LastName = "User",
            Email = "duplicate@acme.com", // same email as existing user
            DateOfBirth = new DateTime(1995, 1, 1)
        };

        // Act
        var result = await controller.CreateUser(customer.Id, input);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetUser_WrongCustomerId_ReturnsNotFound()
    {
        // Arrange: user belongs to customer A, but we ask for it under customer B
        var context = CreateInMemoryContext();
        var customerA = new Customer { Name = "Customer A", Website = null };
        var customerB = new Customer { Name = "Customer B", Website = null };
        context.Customers.AddRange(customerA, customerB);
        await context.SaveChangesAsync();

        var user = new User
        {
            DisplayName = "some.user",
            FirstName = "Some",
            LastName = "User",
            Email = "some@a.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            CustomerId = customerA.Id
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new UserController(context);

        // Act: asking for the user through the WRONG customer's URL
        var result = await controller.GetUser(customerB.Id, user.Id);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result.Result);
    }
}
