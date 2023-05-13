namespace EcommerceApp.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; } 
        public string UserId { get; set; } 
        public string Names { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public int CVV { get; set; }
    }
}
