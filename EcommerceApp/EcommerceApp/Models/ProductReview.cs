using EcommerceApp.Areas.Identity.Data;

namespace EcommerceApp.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Grade { get; set; }
        public string? Description { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ProductsViewModel Product { get; set; }
    }

}
