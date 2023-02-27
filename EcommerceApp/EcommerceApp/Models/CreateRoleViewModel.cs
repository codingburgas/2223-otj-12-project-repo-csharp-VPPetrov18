using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
