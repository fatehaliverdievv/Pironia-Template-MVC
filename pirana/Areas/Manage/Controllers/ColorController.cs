using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pirana.DAL;
using pirana.Models;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]

    [Authorize(Roles = "Admin,Moderator")]
    public class ColorController : Controller
    {
        readonly AppDbContext _context;
        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Colors.ToList());
        }
        public IActionResult Delete(int id)
        {
            Color color = _context.Colors.Find(id);
            if (color is null) return NotFound();
            _context.Colors.Remove(color);
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
            _context.Colors.Add(new Color { Name = name});
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Color color = _context.Colors.Find(id);
            if (color is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(color);
        }
        [HttpPost]
        public IActionResult Update(int? id, Color color)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != color.Id) BadRequest();
            Color existedcolor = _context.Colors.Find(id);
            if (existedcolor is null) return NotFound();
            existedcolor.Name = color.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
