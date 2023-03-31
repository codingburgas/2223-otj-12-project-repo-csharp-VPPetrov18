using EcommerceApp.Areas.Identity.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            List<ProductsViewModel> products = _context.ApplicationProducts.Select(p => new ProductsViewModel
            {
                Id=p.Id,
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
    }

}
