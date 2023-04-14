using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EcommerceApp.Areas.Identity.Data;
using EcommerceApp.Controllers;
using EcommerceApp.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace EcommerceApp.Tests.Controllers
{
    public class AdministrationControllerTests
    {
        private readonly RoleManager<IdentityRole> _fakeRoleManager;
        private readonly UserManager<ApplicationUser> _fakeUserManager;
        private readonly SignInManager<ApplicationUser> _fakeSignInManager;
        private readonly AdministrationController _controller;

        public AdministrationControllerTests()
        {
            _fakeRoleManager = A.Fake<RoleManager<IdentityRole>>();
            _fakeUserManager = A.Fake<UserManager<ApplicationUser>>();
            _fakeSignInManager = A.Fake<SignInManager<ApplicationUser>>();

            _controller = new AdministrationController(_fakeRoleManager, _fakeUserManager, _fakeSignInManager);
        }

        [Fact]
        public async Task DeleteUser_WithExistingUser_ShouldDeleteUserAndRedirectToListUsers()
        {
            // Arrange
            var existingUserId = "existing-user-id";
            var existingUser = new ApplicationUser { Id = existingUserId };
            A.CallTo(() => _fakeUserManager.FindByIdAsync(existingUserId)).Returns(existingUser);
            A.CallTo(() => _fakeUserManager.DeleteAsync(existingUser)).Returns(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUser(existingUserId);

            // Assert
            A.CallTo(() => _fakeUserManager.FindByIdAsync(existingUserId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeUserManager.DeleteAsync(existingUser)).MustHaveHappenedOnceExactly();
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("ListUsers");
        }

        [Fact]
        public async Task DeleteRole_WithExistingRole_ShouldDeleteRoleAndRedirectToListRoles()
        {
            // Arrange
            var existingRoleId = "1";
            var existingRole = new IdentityRole { Id = existingRoleId };
            A.CallTo(() => _fakeRoleManager.FindByIdAsync(existingRoleId)).Returns(existingRole);
            A.CallTo(() => _fakeRoleManager.DeleteAsync(existingRole)).Returns(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteRole(existingRoleId);

            // Assert
            A.CallTo(() => _fakeRoleManager.FindByIdAsync(existingRoleId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeRoleManager.DeleteAsync(existingRole)).MustHaveHappenedOnceExactly();
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectResult.ActionName.Should().Be("ListRoles");
        }
    }
}
