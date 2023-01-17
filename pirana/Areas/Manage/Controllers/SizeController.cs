using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pirana.DAL;
using pirana.Models;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class SizeController : Controller
    {
        readonly AppDbContext _context;
        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Sizes.ToList());
        }
        public IActionResult Delete(int id)
        {
            Size size = _context.Sizes.Find(id);
            if (size is null) return NotFound();
            _context.Sizes.Remove(size);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(string name)
        {
            if (!ModelState.IsValid) return View();
            _context.Sizes.Add(new Size { Name = name});
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Size size = _context.Sizes.Find(id);
            if (size is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(size);
        }
        [HttpPost]
        public IActionResult Update(int? id, Size size)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != size.Id) BadRequest();
            Size existedsize = _context.Sizes.Find(id);
            if (existedsize is null) return NotFound();
            existedsize.Name = size.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
