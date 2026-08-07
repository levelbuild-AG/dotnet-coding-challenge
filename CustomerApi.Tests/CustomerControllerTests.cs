using Microsoft.EntityFrameworkCore;
using CustomerApi.Controllers;
using CustomerApi.Data;
using CustomerApi.DTOs;
using CustomerApi.Models;
using Xunit;

namespace CustomerApi.Tests;

public class CustomerControllerTests
{
    // Helper: builds a brand new, empty fake database for each test.
    // Using a random name means every test gets its own isolated database,
    // so tests never interfere with each other.
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateCustomer_ValidInput_ReturnsCreatedCustomer()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var controller = new CustomerController(context);
        var input = new CreateCustomerDto { Name = "Test Customer", Website = "https://test.com" };

        // Act
        var result = await controller.CreateCustomer(input);

        // Assert
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(result.Result);
        var customer = Assert.IsType<CustomerDto>(createdResult.Value);
        Assert.Equal("Test Customer", customer.Name);
        Assert.True(customer.Id > 0); // the database should have assigned a real id
    }

    [Fact]
    public async Task CreateCustomer_DuplicateName_ReturnsConflict()
    {
        // Arrange: put an existing customer named "Acme" straight into the fake database
        var context = CreateInMemoryContext();
        context.Customers.Add(new Customer { Name = "Acme", Website = null });
        await context.SaveChangesAsync();

        var controller = new CustomerController(context);
        var input = new CreateCustomerDto { Name = "Acme", Website = "https://different.com" };

        // Act
        var result = await controller.CreateCustomer(input);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetCustomer_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var controller = new CustomerController(context);

        // Act
        var result = await controller.GetCustomer(999);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteCustomer_CustomerWithUsers_DeletesUsersToo()
    {
        // Arrange: create a customer AND a user belonging to it
        var context = CreateInMemoryContext();
        var customer = new Customer { Name = "Acme", Website = null };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        context.Users.Add(new User
        {
            DisplayName = "test.user",
            FirstName = "Test",
            LastName = "User",
            Email = "test@acme.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            CustomerId = customer.Id
        });
        await context.SaveChangesAsync();

        var controller = new CustomerController(context);

        // Act
        await controller.DeleteCustomer(customer.Id);

        // Assert: the user should be gone too
        // Note: EF Core InMemory doesn't enforce real database cascade rules automatically,
        // so this test specifically checks OUR controller behavior, not the database engine.
        var remainingUsers = context.Users.Where(u => u.CustomerId == customer.Id).ToList();
        Assert.Empty(remainingUsers);
    }
}
