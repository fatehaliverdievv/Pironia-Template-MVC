using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pirana.DAL;
using pirana.ViewModels;

namespace pirana.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public  async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderVM headerVM = new HeaderVM();
            headerVM.Settings = _context.Settings.ToDictionary(s => s.Key, s => s.Value);
            headerVM.Basket = GetBasket();
            return View(headerVM);  
        }
        BasketVM GetBasket()
        {
            BasketVM basketVM = new BasketVM();
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["basket"]))
            {
                basketItems= JsonConvert.DeserializeObject<List<BasketItemVM>>(HttpContext.Request.Cookies["basket"]);
            }
            if(basketItems != null)
            {
                basketVM.Products = new List<ProductBasketItemVm>();
                foreach (var item in basketItems)
                {
                    ProductBasketItemVm productBasket=new ProductBasketItemVm();
                    productBasket.Product=_context.Products.Include(p => p.ProductImages).FirstOrDefault(p=>p.Id==item.Id);
                    productBasket.Count = item.Count;
                    basketVM.Products.Add(productBasket);
                    basketVM.TotalPrice+= productBasket.Product.SellPrice * productBasket.Count;
                }
            }
            return basketVM;
        }
    }
}
