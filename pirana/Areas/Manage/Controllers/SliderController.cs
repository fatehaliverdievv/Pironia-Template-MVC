using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.VisualStudio;
using pirana.DAL;
using pirana.Models;
using pirana.ViewModels;
namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class SliderController : Controller
    {
        readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.ToList());
        }
        public IActionResult Delete(int id)
        {
            Slider slider = _context.Sliders.Find(id);
            if (slider is null) return NotFound();
            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Slider slider = _context.Sliders.Find(id);
            if (slider is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(slider);
        }
        [HttpPost]
        public IActionResult Update(int? id, UpdateSliderVm sliderVm)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != sliderVm.Id) BadRequest();
            Slider existedslider = _context.Sliders.Find(id);
            if (existedslider is null) return NotFound();
            Slider anotherSlider=_context.Sliders.FirstOrDefault(s=>s.Order == sliderVm.Order);
            if (anotherSlider != null)
            {
                anotherSlider.Order = _context.Sliders.Find(id).Order;
            }
            IFormFile file = sliderVm.Image;
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
            using (var stream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath , "assets","images", fileName
                ), FileMode.Create))
            {
                file.CopyTo(stream);
            }
            existedslider.Title = sliderVm.Title;
            existedslider.SecondaryTitle = sliderVm.SecondaryTitle;
            existedslider.ButtonName = sliderVm.ButtonName;
            existedslider.Description = sliderVm.Description;
            existedslider.ImgUrl = fileName;
            existedslider.Order= sliderVm.Order;
            
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateSliderVm sliderVm)
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = sliderVm.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "yuklediyiniz fayl shekil deyil :F");
                return View();
            }
            if(file.Length> 400 * 1024)
            {
                ModelState.AddModelError("Image", "shekil 400kbdan artiq olmamalidir.");
                return View();
            }
            string fileName = Guid.NewGuid() + file.FileName;
           using(var stream = new FileStream("C:\\Users\\Fateh\\Desktop\\C#\\pirana\\pirana\\wwwroot\\assets\\images\\"+ fileName ,FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Slider slider = new Slider { Description = sliderVm.Description, ButtonName = sliderVm.ButtonName, Title = sliderVm.Title, SecondaryTitle = sliderVm.SecondaryTitle, ImgUrl=fileName, Order=sliderVm.Order};
            _context.Sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
