using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace pirana.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public string SecondaryTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonName {get; set; }

        public string ImgUrl { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public int Order { get; set; }
    }
}
