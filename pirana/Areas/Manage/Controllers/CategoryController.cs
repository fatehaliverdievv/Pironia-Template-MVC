using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pirana.DAL;
using pirana.Models;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class CategoryController : Controller
    {
        readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Categories.ToList());
        }
        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.Find(id);
            if (category is null) return NotFound();
            _context.Categories.Remove(category);
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
            _context.Categories.Add(new Category { Name = name});
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Category category = _context.Categories.Find(id);
            if (category is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(int? id, Category category)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != category.Id) BadRequest();
            Category existedcategory = _context.Categories.Find(id);
            if (existedcategory is null) return NotFound();
            existedcategory.Name = category.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
