using EcommerceApp.Areas.Identity.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor that accepts an instance of the ApplicationDbContext class, which is used to communicate with the database.
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method that retrieves all the products from the database and passes them to the Products view.
        public ActionResult Products()
        {
            // Retrieve all the products from the database.
            List<ProductsViewModel> products;

            // Use the 'using' statement to ensure proper disposal of the database context.
            using (var context = _context)
            {
                products = _context.ApplicationProducts.Select(p => new ProductsViewModel
                {
                    Id = p.Id,
                    ImageTitle = p.ImageTitle,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    SizeS = p.SizeS,
                    SizeM = p.SizeM,
                    SizeL = p.SizeL,
                    SizeXL = p.SizeXL,
                    Color = p.Color,
                    Rating = p.Rating,
                    Price = p.Price
                }).ToList();

                // Pass the list of products to the Products view.
                return View(products);
            }
        }


        // Action method that retrieves a product from the database based on its ID and passes it to the ProductPage view.
        public IActionResult ProductPage(int id)
        {
            // Retrieve the product with the specified ID from the database.
            var product = _context.ApplicationProducts.FirstOrDefault(p => p.Id == id);

            // Check if the product was found.
            if (product == null)
            {
                // Return a 404 error.
                return NotFound();
            }

            // Create a view model to pass to the view.
            var viewModel = new ProductsViewModel
            {
                Id = product.Id,
                ImageTitle = product.ImageTitle,
                ProductName = product.ProductName,
                Description = product.Description,
                SizeS = product.SizeS,
                SizeM = product.SizeM,
                SizeL = product.SizeL,
                SizeXL = product.SizeXL,
                Color = product.Color,
                Rating = product.Rating,
                Price = product.Price
            };

            // Pass the view model to the ProductPage view.
            return View(viewModel);
        }




        // Action method that adds a new product review to the database.
        [HttpPost]
        public IActionResult SubmitProductReview(int productId, int grade, string description)
        {
            // Get the current user's ID.
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user has already reviewed this product.
            var existingReview = _context.ProductReviews.FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);

            if (existingReview != null)
            {
                // User has already reviewed this product, return an error message or redirect to an error page.
                return BadRequest(new { message = "You have already reviewed this product." });
            }

            // Create a new ProductReview instance with the data submitted by the user.
            var review = new ProductReview
            {
                UserId = userId,
                ProductId = productId,
                Grade = grade,
                Description = description
            };

            // Add the new review to the database and save changes.
            _context.ProductReviews.Add(review);
            _context.SaveChanges();


            // Retrieve all reviews for the current product.
            List<ProductReview> reviews = _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToList();

            // Calculate the average of the grades.
            double averageGrade = reviews.Average(r => r.Grade);

            // Update the rating field in the ApplicationProducts table.
            var product = _context.ApplicationProducts
                .Where(p => p.Id == productId)
                .FirstOrDefault();

            if (product != null)
            {
                averageGrade = (double)System.Math.Round(averageGrade, 2);
                product.Rating = averageGrade;
                _context.SaveChanges();
            }


            return RedirectToAction("ProductPage", new { id = productId });
        }



        public ActionResult Sizing()
        {
            return View();
        }


        public ActionResult Policies()
        {
            return View();
        }


        // This method is used to retrieve all the reviews for a specific product.
        public IActionResult Reviews(int productId)
        {
            // The method uses LINQ to Entities to query the ProductReviews table of the database
            // through the ApplicationDbContext instance _context.
            var reviews = _context.ProductReviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .ToList();

            return View(reviews);
        }

    }

}
