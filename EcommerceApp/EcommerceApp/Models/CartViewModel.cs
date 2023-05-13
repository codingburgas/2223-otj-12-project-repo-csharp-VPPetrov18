using EcommerceApp.Areas.Identity.Data;

namespace EcommerceApp.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string ImageTitle { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ProductsViewModel Product { get; set; }
        public CartViewModel()
        {
            Quantity = 1;
        }
    }
}
