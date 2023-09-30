using System.ComponentModel.DataAnnotations;

namespace Product.API.DTOs.Product
{
    public class CreateProductDto
    {
        [Required]
        public string No { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Maximum length for Product Name is 250 characters")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "Maximum length for Product Summary is 250 characters")]
        public string Summary { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
