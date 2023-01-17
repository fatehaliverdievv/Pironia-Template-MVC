namespace pirana.ViewModels
{
    public class BasketVM
    {
        public ICollection<ProductBasketItemVm> Products { get; set; }
        public double TotalPrice{ get; set; }
    }
}
