namespace EcommerceApp.Models
{
    
    public class ProductsViewModel
    {
        public int Id { get; set; }
        public string ImageTitle { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }

        public bool SizeS { get; set; }
        public bool SizeM { get; set; }
        public bool SizeL { get; set; }
        public bool SizeXL { get; set; }
    
        public string Color { get; set; }
        public double Rating { get; set; }
        public float Price { get; set; }
    }
}
