using pirana.Models;

namespace pirana.ViewModels
{
    public class UpdateImageVM
    {
        public int Id { get; set; }
        public IFormFile? CoverImage { get; set; }
        public IFormFile? HoverImage { get; set; }
        public List<IFormFile>? OtherImages { get; set; }
        public IEnumerable<ProductImage>? ProductImages { get; set; }
        public List<int>? ImageIds { get; set; }
    }
}
