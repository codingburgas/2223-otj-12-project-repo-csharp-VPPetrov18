using EcommerceApp.Areas.Identity.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;

namespace EcommerceApp.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Find the user by id using the user manager
            var user = await userManager.FindByIdAsync(id);

            // If user is not found, show an error message and return NotFound view
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            // If user is found, attempt to delete the user using the user manager
            else
            {
                var result = await userManager.DeleteAsync(user);

                // If user is successfully deleted, redirect to ListUsers action
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                // If there are errors in deleting the user, add them to the model state and return ListUsers view
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            // Find the role by id using the role manager
            var role = await roleManager.FindByIdAsync(id);

            // If role is not found, show an error message and return NotFound view
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            // If role is found, attempt to delete the role using the role manager
            else
            {
                var result = await roleManager.DeleteAsync(role);

                // If role is successfully deleted, redirect to ListRoles action
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                // If there are errors in deleting the role, add them to the model state and return ListRoles view
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRoles");
            }
        }



        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            // Find the user by id using the user manager
            var user = await userManager.FindByIdAsync(id);

            // If user is not found, show an error message and return NotFound view
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // Get user claims and roles
            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            // Create the view model with the user information
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles
            };

            return View(model);
        }

        

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            // Retrieve the input values from the form
            var adminEmail = Request.Form["adminEmail"];
            var adminPassword = Request.Form["adminPassword"];
            var userId = Request.Form["userId"];
            var userFirstName = Request.Form["userFirstName"];
            var userLastName = Request.Form["userLastName"];
            var userEmail = Request.Form["userEmail"];

            // Check if the current user is an admin
            var currentUser = await userManager.GetUserAsync(User);
            if (!await userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return Forbid(); // Return 403 Forbidden if the current user is not an admin
            }

            // Check if the provided admin email and password are correct
            var resultAdmin = await signInManager.PasswordSignInAsync(adminEmail, adminPassword, false, lockoutOnFailure: false);
            if (!resultAdmin.Succeeded)
            {
                return BadRequest(new { message = "Invalid admin email or password" }); // Return 400 Bad Request if the provided admin email or password are incorrect
            }


            // Retrieve the user we want to edit
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                // Update the user information
                user.Email = userEmail;
                user.UserName = userEmail;
                user.FirstName = userFirstName;
                user.LastName = userLastName;

                // Update the user in the database
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                // If there were errors, add them to the ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }



        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new IdentityRole with the name specified in the model
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                // Create the role using the RoleManager
                IdentityResult result = await roleManager.CreateAsync(identityRole);


                if (result.Succeeded)
                {
                    // If the role was successfully created, redirect to the list of roles
                    return RedirectToAction("ListRoles", "Administration");
                }

                // If the creation was not successful, add the error messages to the ModelState
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // If the model state is not valid, return the view with the model
            return View(model);
        }



        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }




        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role with the specified id
            var role = await roleManager.FindByIdAsync(id);
        
            if(role == null)
            {
                // If the role is not found, return the NotFound view
                ViewBag.ErrorMessage = $"Role with Id= {id} cannot be found";
                return View("NotFound");
            }

            // Create a new EditRoleViewModel with the role's id and name
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Get all the users and add the usernames of those who are in the role to the model's Users list
            foreach (var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            // Return the view with the model
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            // Check if role exists
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id= {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                // Update the role's name
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if(result.Succeeded)
                {
                    // If update was successful, redirect to ListRoles action
                    return RedirectToAction("ListRoles"); 
                }

                // If update failed, add model errors and return view with model data
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model); 
            }

           
        }



        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            // Set ViewBag property to the roleId for later use in the view
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            // Check if role exists
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("Not Found");
            }

            // Create a list of UserRoleViewModel objects to display in the view
            var model = new List<UserRoleViewModel>();

            foreach(var user in userManager.Users)
            {
                // Create a new UserRoleViewModel for each user
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                // Set the isSelected property based on whether the user is in the selected role
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.isSelected = true;
                }
                else
                {
                    userRoleViewModel.isSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            // Return the view with the list of UserRoleViewModel objects
            return View(model); 
        }



        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            // Check if role exists
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            // Loop through the list of UserRoleViewModel objects
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                // If user is selected and not already in the role, add them to the role
                if (model[i].isSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                // If user is not selected and is in the role, remove them from the role
                else if (!model[i].isSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                // If the user was successfully added or removed from the role, redirect to EditRole action
                if (result.Succeeded) 
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }

            }
            
            return RedirectToAction("EditRole", new {Id=roleId});
        }
    }
}
