using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pirana.DAL;
using pirana.Models;
using pirana.ViewModels;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class BrandController : Controller
    {
        readonly AppDbContext _context;
        public BrandController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Brands.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateBrandVm brandVm )
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = brandVm.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "yuklediyiniz fayl shekil deyil :F");
                return View();
            }
            if (file.Length > 1 * 1024 * 1024)
            {
                ModelState.AddModelError("Image", "shekil 1mbdan artiq olmamalidir.");
                return View();
            }
            string fileName = Guid.NewGuid() + file.FileName;
            using (var stream = new FileStream("C:\\Users\\Fateh\\Desktop\\C#\\pirana\\pirana\\wwwroot\\assets\\images\\" + fileName, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Brand brand = new Brand { LogoUrl = fileName };
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Brand brand = _context.Brands.Find(id);
            if (brand is null) return NotFound();
            _context.Brands.Remove(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Brand brand = _context.Brands.Find(id);
            if (brand is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(brand);
        }
        [HttpPost]
        public IActionResult Update(int? id, Brand brand)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != brand.Id) BadRequest();
            Brand existedbrand = _context.Brands.Find(id);
            if (existedbrand is null) return NotFound();
            existedbrand.LogoUrl = brand.LogoUrl;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
