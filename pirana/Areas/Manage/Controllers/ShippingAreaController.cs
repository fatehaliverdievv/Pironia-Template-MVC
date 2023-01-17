using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pirana.DAL;
using pirana.Models;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class ShippingAreaController : Controller
    {
        readonly AppDbContext _context;
        public ShippingAreaController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.ShippingAreas.ToList());
        }
        public IActionResult Delete(int id)
        {
            ShippingArea area = _context.ShippingAreas.Find(id);
            if (area is null) return NotFound();
            _context.ShippingAreas.Remove(area);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            ShippingArea area = _context.ShippingAreas.Find(id);
            if (area is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(area);
        }
        [HttpPost]
        public IActionResult Update(int? id, ShippingArea shippingArea)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != shippingArea.Id) BadRequest();
            ShippingArea existedarea = _context.ShippingAreas.Find(id);
            if (existedarea is null) return NotFound();
            existedarea.Name = shippingArea.Name;
            existedarea.Info = shippingArea.Info;
            existedarea.LogoUrl = shippingArea.LogoUrl;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            ICollection<ShippingArea> shippingAreas = _context.ShippingAreas.ToList();
            if (shippingAreas.Count>=3)
            {
                return RedirectToAction(nameof(Index)); 
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Create(string name, string info, string logourl)
        {
            if (!ModelState.IsValid) return View();
            _context.ShippingAreas.Add(new ShippingArea { Name = name, Info = info, LogoUrl = logourl});
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }   
    }
}
