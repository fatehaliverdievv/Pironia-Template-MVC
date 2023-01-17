using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pirana.Abstraction;
using pirana.DAL;
using pirana.Models;
using pirana.ViewModels;
using System.ComponentModel;
using System.Diagnostics;

namespace pirana.Controllers
{
    public class HomeController : Controller
    {
        readonly AppDbContext _context;
        readonly IEmailService _emailService;

        public HomeController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            ViewBag.Banners = _context.Banners.ToList();
             IQueryable<Product> products= _context.Products.Where(p => !p.IsDeleted).Include(p => p.ProductImages).Take(4).AsQueryable();
            HomeVM home = new HomeVM { Banners = _context.Banners.ToList(), Sliders = _context.Sliders.ToList(), Brands = _context.Brands.ToList(), Products = _context.Products.Include(p => p.ProductImages).Where(p=>p.IsDeleted==false).ToList(), Clients = _context.Clients.ToList(), FeaturedProducts = products, LastestProducts = products.OrderByDescending(p => p.Id)};
            return View(home);
        }
        public IActionResult Shop()
        {
            int? ColorProductCount=0;
            int? CategoryProductCount=0;
            List<Category> categories = _context.Categories.ToList();
            List<Color> colors = _context.Colors.ToList();
            List<Product> products = _context.Products.Include(p => p.ProductImages).Where(p => p.IsDeleted == false).ToList();
            ViewBag.Categories = categories;
            ViewBag.Color = colors;
            ViewBag.ColorProductCount= ColorProductCount;
            ViewBag.CategoryProductCount = CategoryProductCount;
            return View(products);
        }
        public IActionResult SingleProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoadProducts(int skip = 4, int take = 4)
        {
            return PartialView("_ProductPartial", _context.Products.Where(p => !p.IsDeleted).Include(p => p.ProductImages).Skip(skip).Take(take));
        }
        public IActionResult SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
            return Content("Ok");
        }
        public IActionResult GetSession(string key)
        {
            string value = HttpContext.Session.GetString(key);
            return Content(value);
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult SetCookie(string key,string value)
        {
            HttpContext.Response.Cookies.Append(key,value, new CookieOptions
            {
                MaxAge=TimeSpan.FromDays(1)
            });
            return Content("Okay");
        }
        public IActionResult GetCookie(string key)
        {
            string value = HttpContext.Request.Cookies[key];
            return Content(value);
        }
        public IActionResult AddBasket(int? id)
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["basket"]))
            {
                items = JsonConvert.DeserializeObject<List<BasketItemVM>>(HttpContext.Request.Cookies["basket"]);
            }
            BasketItemVM itemVM = items.FirstOrDefault(i => i.Id == id);
            if(itemVM == null)
            {
                itemVM = new BasketItemVM();
                itemVM.Id = (int)id;
                itemVM.Count = 1;
                items.Add(itemVM);
            }
            else
            {
                itemVM.Count++;
            }
            string basket = JsonConvert.SerializeObject(items);
            HttpContext.Response.Cookies.Append("basket", basket,new CookieOptions
            {
                MaxAge = TimeSpan.FromDays(5)
            });
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetViewComponent(int id)
        {
            return ViewComponent("QuickView", id);
        }
        public IActionResult SendEmail()
        {
            _emailService.Send("tu7l7qsfn", "salam", "necesen", false);
            return RedirectToAction("Index","Home");
        }
    }
}