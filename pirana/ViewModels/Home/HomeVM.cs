using pirana.Models;

namespace pirana.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Color> Colors { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Product> LastestProducts { get; set; }
        public IEnumerable<Product> FeaturedProducts { get; set; }
        public IEnumerable<ShippingArea> ShippingAreas { get; set; }
        public IEnumerable<Slider> Sliders { get; set; }
    }
}
