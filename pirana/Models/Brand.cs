using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations.Schema;

namespace pirana.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string LogoUrl { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
