using System.ComponentModel.DataAnnotations.Schema;

namespace pirana.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SecondaryTitle { get; set; }
        public string ImgUrl { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }


    }
}
