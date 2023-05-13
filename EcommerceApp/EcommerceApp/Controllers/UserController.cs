using EcommerceApp.Areas.Identity.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Policy;
using System.Web;
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

        public IActionResult AddToCart(int productId, string size, string imageTitle, string productName, float price)
        {
            productName= HttpUtility.HtmlDecode(productName);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve the user's current cart from the database
            var userCart = _context.CartItems
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId && c.Size== size);

            if (userCart == null)
            {
                // If the user's cart doesn't exist yet, create a new one
                userCart = new CartViewModel
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    Size = size,
                    ImageTitle = imageTitle,
                    Price = price,
                    ProductName = productName
                };

                _context.CartItems.Add(userCart);
            }
            else
            {
                // If the user's cart already exists, update the quantity
                userCart.Quantity += 1;
                _context.CartItems.Update(userCart);
            }

            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        public IActionResult Cart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve all the items in the user's cart from the database
            var cartItems = _context.CartItems
                .Where(c => c.UserId == userId)
                .ToList();

            // Create a new list to store the cart items
            var cartList = new List<CartViewModel>();

            // Loop through the cart items and add them to the list
            foreach (var item in cartItems)
            {
                var cartItem = new CartViewModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Size = item.Size,
                    ImageTitle = item.ImageTitle,
                    Price = item.Price,
                    ProductName = item.ProductName
                };

                cartList.Add(cartItem);
            }
            

            return View(cartList);
        }

        public IActionResult RemoveFromCart(int id)
        {

            // Find the cart item with the specified id.
            var cartItem = _context.CartItems.FirstOrDefault(c => c.Id == id);

            // Check if the cart item was found.
            if (cartItem == null)
            {
                // Return a 404 error.
                return NotFound();
            }

            // Remove the cart item from the database and save changes.
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            // Redirect to the Cart page.
            return RedirectToAction("Cart");
        }


       
        public IActionResult UpdateQuantityCartItem(int id, int quantity)
        {
            var cartItem = _context.CartItems.FirstOrDefault(item => item.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

       

        public IActionResult Checkout(float price, int itemCount)
        {
            ViewData["Price"] = price;
            ViewData["ItemCount"] = itemCount;
            return View();
        }


        public IActionResult SaveOrderDetails(string firstname, string email, string address,
            string city, string state, string zip, string cardname, string cardnumber, string expmonth,
            string expyear, int cvv)
        {
            if (string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address) ||
        string.IsNullOrEmpty(city) || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(zip) ||
        string.IsNullOrEmpty(cardname) || string.IsNullOrEmpty(cardnumber) || string.IsNullOrEmpty(expmonth) ||
        string.IsNullOrEmpty(expyear) || cvv == 0)
            {
                TempData["ErrorMessage"] = "Моля попълнете всички полета";
                
                return RedirectToAction("Checkout");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = new OrderViewModel
            {
                Names = firstname,
                Email = email,
                Address = address,
                City = city,
                State = state,
                Zip = zip,
                CardName = cardname,
                CardNumber = cardnumber,
                ExpMonth = expmonth,
                ExpYear = expyear,
                CVV = cvv,
                UserId = userId
            };
            _context.Order.Add(order);
            _context.SaveChanges();


            return RedirectToAction("Order");
        }


        public IActionResult Order()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve all the items in the user's cart from the database
            var orderDetails = _context.Order
                .Where(c => c.UserId == userId)
                .ToList();

            // Create a new list to store the cart items
            var orderList = new List<OrderViewModel>();

            // Loop through the cart items and add them to the list
            foreach (var item in orderDetails)
            {
                var orderDetail = new OrderViewModel
                {
                    Id=item.Id,
                    Names = item.Names,
                    Email = item.Email,
                    Address = item.Address,
                    City = item.City,
                    State = item.State,
                    Zip = item.Zip,
                    CardName = item.CardName,
                    CardNumber = item.CardNumber,
                    ExpMonth = item.ExpMonth,
                    ExpYear = item.ExpYear,
                    CVV = item.CVV,
                    UserId = item.UserId
                };

                orderList.Add(orderDetail);
            }


            return View(orderList);
        }

        public IActionResult RemoveAnOrderDetail(int id)
        {

            // Find the cart item with the specified id.
            var orderDetail = _context.Order.FirstOrDefault(c => c.Id == id);

            // Check if the cart item was found.
            if (orderDetail == null)
            {
                // Return a 404 error.
                return NotFound();
            }

            // Remove the cart item from the database and save changes.
            _context.Order.Remove(orderDetail);
            _context.SaveChanges();

            // Redirect to the Cart page.
            return RedirectToAction("Order");
        }

    }

}
