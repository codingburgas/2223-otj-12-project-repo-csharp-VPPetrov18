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

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult Products()
        {
            List<ProductsViewModel> products;

            // The 'using' statement ensures proper disposal of the database context, including closing open connections and
            // returning them to the connection pool. This helps prevent exhaustion of the pool and ensures proper resource management.
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

                return View(products);
            }
        }


        public IActionResult ProductPage(int id)
        {
            var product = _context.ApplicationProducts.FirstOrDefault(p => p.Id == id);

            // check if product was found
            if (product == null)
            {
                return NotFound();
            }

            // create a view model to pass to the view
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

            // pass the view model to the view
            return View(viewModel);
        }





        [HttpPost]
        public IActionResult SubmitProductReview(int productId, int grade, string description)
        {
            // get the current user's id
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // check if the user has already reviewed this product
            var existingReview = _context.ProductReviews.FirstOrDefault(r => r.ProductId == productId && r.UserId == userId);

            if (existingReview != null)
            {
                // user has already reviewed this product, return an error message or redirect to an error page
                return BadRequest(new { message = "You have already reviewed this product." });
            }

            // create a new ProductReview instance with the data submitted by the user
            var review = new ProductReview
            {
                UserId = userId,
                ProductId = productId,
                Grade = grade,
                Description = description
            };

            // add the new review to the database and save changes
            _context.ProductReviews.Add(review);
            _context.SaveChanges();


            // Retrieve all reviews for the current product
            List<ProductReview> reviews = _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToList();

            // Calculate the average of the grades
            double averageGrade = reviews.Average(r => r.Grade);

            // Update the rating field in the ApplicationProducts table
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


        public IActionResult Reviews(int productId)
        {
            var reviews = _context.ProductReviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .ToList();

            return View(reviews);
        }

    }

}
