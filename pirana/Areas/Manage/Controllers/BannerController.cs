using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pirana.DAL;
using pirana.Models;
using pirana.Utilies.Roles;
using pirana.ViewModels;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class BannerController : Controller
    {
        readonly AppDbContext _context;
        public BannerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Banners.ToList());
        }
        public IActionResult Delete(int id)
        {
            Banner banner = _context.Banners.Find(id);
            if (banner is null) return NotFound();
            _context.Banners.Remove(banner);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Banner banner = _context.Banners.Find(id);
            if (banner is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(banner);
        }
        [HttpPost]
        public IActionResult Update(int? id, Banner banner)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != banner.Id) BadRequest();
            Banner existedbanner = _context.Banners.Find(id);
            if (existedbanner is null) return NotFound();
            IFormFile file = banner.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "yuklediyiniz fayl shekil deyil :F");
                return View();
            }
            if (file.Length > 400 * 1024)
            {
                ModelState.AddModelError("Image", "shekil 400kbdan artiq olmamalidir.");
                return View();
            }
            string fileName = Guid.NewGuid() + file.FileName;
            using (var stream = new FileStream("C:\\Users\\Fateh\\Desktop\\C#\\pirana\\pirana\\wwwroot\\assets\\images\\" + fileName, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            banner.Title = banner.Title;
            banner.SecondaryTitle = banner.SecondaryTitle;
            banner.ImgUrl = fileName;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            ICollection<Banner> banners = _context.Banners.ToList();
            if (banners.Count >= 4)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Create(CreateBannerVm bannerVm)
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = bannerVm.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "yuklediyiniz fayl shekil deyil :F");
                return View();
            }
            if (file.Length > 400 * 1024)
            {
                ModelState.AddModelError("Image", "shekil 400kbdan artiq olmamalidir.");
                return View();
            }
            string fileName = Guid.NewGuid() + file.FileName;
            using (var stream = new FileStream("C:\\Users\\Fateh\\Desktop\\C#\\pirana\\pirana\\wwwroot\\assets\\images\\" + fileName, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Banner banner = new Banner {Title = bannerVm.Title, SecondaryTitle = bannerVm.SecondaryTitle, ImgUrl = fileName};
            _context.Banners.Add(banner);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
