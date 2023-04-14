using EcommerceApp.Areas.Identity.Data;
using EcommerceApp.Controllers;
using EcommerceApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Xunit;
namespace EcommerceApp.Tests.ControllerTests
{
    public class UserControllerTests
    {
        [Fact]
        public void Products_Returns_View_With_Empty_List_When_No_Products_Exist()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var fakeDbContext = new ApplicationDbContext(dbContextOptions);
            var controller = new UserController(fakeDbContext);

            // Act
            var result = controller.Products() as ViewResult;
            var actualProducts = result?.Model as List<ProductsViewModel>;

            // Assert
            actualProducts.Should().NotBeNull();
            actualProducts.Should().BeEmpty();
        }
        [Fact]
        public void ProductPage_Returns_NotFound_When_Product_Does_Not_Exist()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var fakeDbContext = new ApplicationDbContext(dbContextOptions);
            var controller = new UserController(fakeDbContext);

            // Act
            var result = controller.ProductPage(-1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        
    }
}
