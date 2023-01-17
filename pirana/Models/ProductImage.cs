namespace pirana.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public bool? IsCover { get; set; }
        public string ImgUrl { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
