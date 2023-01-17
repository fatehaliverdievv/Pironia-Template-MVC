using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace pirana.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, Double.MaxValue)]
        public double SellPrice { get; set; }
        [Range(0.0,Double.MaxValue)]
        public double CostPrice { get; set; }
        [Range(0.0,100)]
        public int Discount { get; set; }
        public string SKU { get; set; }  
        public bool IsDeleted {get; set; }
        public ProductInformation? ProductInformation { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
        public ICollection<ProductColor>? ProductColors { get; set; }
        public ICollection<ProductSize>? ProductSizes { get; set; }
    }
}
