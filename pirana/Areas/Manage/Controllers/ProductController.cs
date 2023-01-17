using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pirana.DAL;
using pirana.Models;
using pirana.Utilies.Extension;
using pirana.ViewModels;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(int page =1)
        {
            PaginateVM<Product> paginateVM = new PaginateVM<Product>();
            paginateVM.PageMaxCount=(int)Math.Ceiling((decimal)_context.Products.Count()/10);
            paginateVM.CurrentPage = page;
            if (page > paginateVM.PageMaxCount || page < 1) return BadRequest();
            if (_context.Products!= null)
            {
                paginateVM.Items = _context.Products.Skip((page-1)*10).Take(10).Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.ProductColors).ThenInclude(p => p.Color).Include(p => p.ProductSizes).ThenInclude(ps => ps.Size).Include(p => p.ProductImages).Where(p => p.IsDeleted == false);
            }
            return View(paginateVM);
        }
        public IActionResult DeletedProduct()
        {
            return View(_context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).Include(p => p.ProductColors).ThenInclude(p => p.Color).Include(p => p.ProductSizes).ThenInclude(ps => ps.Size).Include(p => p.ProductImages).ToList().Where(p => p.IsDeleted == true));
        }
        public IActionResult DeletePermanently(int? id)
        {
            if (id is null || id == 0) return NotFound();
            Product product = _context.Products.Find(id);
            if (product is null) return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(DeletedProduct));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null || id == 0) return NotFound();
            Product existed = _context.Products.Include(p => p.ProductImages).Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefault(p => p.Id == id);
            if (existed == null) return NotFound();
            foreach (ProductImage image in existed.ProductImages)
            {
                image.ImgUrl.DeleteFile(_webHostEnvironment.WebRootPath, "assets/images/product");
                //_context.ProductImages.Remove(image);
            }
            //_context.ProductSizes.RemoveRange(existed.ProductSizes);
            //_context.ProductColors.RemoveRange(existed.ProductColors);
            _context.ProductImages.RemoveRange(existed.ProductImages);
            existed.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories,nameof(Category.Id),nameof(Category.Name));
            ViewBag.Colors = new SelectList(_context.Colors,"Id","Name");
            ViewBag.Sizes = new SelectList(_context.Sizes, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateProductVM productVM)
        {
            var coverImg = productVM.CoverImage;
            var hoverImg = productVM.HoverImage;
            var otherImg = productVM.OtherImages ?? new List<IFormFile>();
            string result = coverImg?.CheckValidate("image/", 300);
            if (result?.Length>0)
            {
                ModelState.AddModelError("CoverImage", result);
            }
            result = hoverImg?.CheckValidate("image/", 300);
            if (result?.Length>0)
            {
                ModelState.AddModelError("HoverImage", result);
            }
            foreach (IFormFile image in otherImg)
            {
                result = image?.CheckValidate("image/", 300);
                if (result?.Length >0)
                {
                    ModelState.AddModelError("OtherImages", result);
                }
            }
            if (productVM.ColorIds != null)
            {
                foreach (int colorid in productVM.ColorIds)
                {
                    if (!_context.Colors.Any(c => c.Id == colorid))
                    {
                        ModelState.AddModelError("ColorIds", "Bele bir reng yoxdu :F");
                        break;
                    }
                }
            }
            if (productVM.CategoryIds != null)
            {
                foreach (int categoryid in productVM.CategoryIds)
                {
                    if (!_context.Categories.Any(c => c.Id == categoryid))
                    {
                        ModelState.AddModelError("CategoryIds", "Bele bir reng yoxdu :F");
                        break;
                    }
                }
            }
            if(productVM.SizeIds != null)
            {
                foreach (int sizeid in productVM.SizeIds)
                {
                    if (!_context.Sizes.Any(s => s.Id == sizeid))
                    {
                        ModelState.AddModelError("SizeIds", "Bele bir reng yoxdu :F");
                        break;
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                ViewBag.Colors = new SelectList(_context.Colors, "Id", "Name");
                ViewBag.Sizes = new SelectList(_context.Sizes, "Id", "Name");
                return View();
            }
            var sizes = _context.Sizes.Where(s=>productVM.SizeIds.Contains(s.Id));
            var categories = _context.Categories.Where(ca=>productVM.CategoryIds.Contains(ca.Id));
            var colors = _context.Colors.Where(co=>productVM.ColorIds.Contains(co.Id));
            Product newProduct = new Product
            {
                Name = productVM.Name,
                CostPrice = productVM.CostPrice, SellPrice = productVM.SellPrice,
                Description = productVM.Description,
                Discount = productVM.Discount,
                IsDeleted = false,
                SKU = "1",
            };
            List<ProductImage> images = new List<ProductImage>();
            if (coverImg != null)
            {
                images.Add(new ProductImage { ImgUrl = coverImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "product")), IsCover = true, Product = newProduct });
            }
            if (hoverImg != null)
            {
                images.Add(new ProductImage { ImgUrl = hoverImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "product")), IsCover = false, Product = newProduct });
            }
            foreach (var item in otherImg)
            {
                images.Add(new ProductImage { ImgUrl = item.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "product")),IsCover=null ,Product=newProduct});
            }
            newProduct.ProductImages = images;
            foreach (var item in colors)
            {
                _context.ProductColors.Add(new ProductColor { Product = newProduct, ColorId = item.Id });
            }
            foreach (var item in categories)
            {
                _context.ProductCategories.Add(new ProductCategory { Product = newProduct, CategoryId = item.Id });
            }
            foreach (var item in sizes)
            {
                _context.ProductSizes.Add(new ProductSize { Product = newProduct, SizeId = item.Id });
            }
            _context.Products.Add(newProduct);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            var categories = _context.ProductCategories.Where(p=>p.ProductId==id);
            var sizes = _context.ProductSizes.Where(s => s.ProductId==id);
            var colors = _context.ProductColors.Where(c => c.ProductId==id);
            ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
            ViewBag.Colors = new SelectList(_context.Colors, nameof(Models.Color.Id), nameof(Models.Color .Name));
            ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Models.Size.Id), nameof(Models.Size.Name));
            if (id is null || id == 0) BadRequest();
            Product product = _context.Products.FirstOrDefault(p=>p.Id==id);
            if (product is null) return NotFound();
            UpdateProductVM productVm = new UpdateProductVM();
            productVm.Id = product.Id;
            productVm.Name = product.Name;
            productVm.SellPrice = product.SellPrice;
            productVm.CostPrice = product.CostPrice;
            productVm.Description = product.Description;
            productVm.Discount = product.Discount;
            productVm.CategoryIds = new List<int>();
            productVm.ColorIds = new List<int>();
            productVm.SizeIds = new List<int>();
            foreach (var item in categories) productVm.CategoryIds.Add(item.CategoryId);
            foreach (var item in sizes) productVm.SizeIds.Add(item.SizeId);
            foreach (var item in colors) productVm.ColorIds.Add(item.ColorId);
            return View(productVm);
        }
        [HttpPost]
        public IActionResult Update(int? id, UpdateProductVM productVM)
        {
            foreach (var item in (productVM.ColorIds ?? new List<int>()))
            {
                if (!_context.Colors.Any(c => c.Id == item))
                {
                    ModelState.AddModelError("ColorIds", "Bele reng yoxdu qardas");
                }
            }
            foreach (var item in (productVM.SizeIds ?? new List<int>()))
            {
                if (!_context.Sizes.Any(s => s.Id == item))
                {
                    ModelState.AddModelError("SizeIds", "Bele bir olcu yoxdu");
                }
            }
            foreach (var item in (productVM.CategoryIds ?? new List<int>()))
            {
                if (!_context.Categories.Any(c => c.Id == item))
                {
                    ModelState.AddModelError("CategoryIds", "Bele bir kateqoriya yoxdu");
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Colors = new SelectList(_context.Colors, nameof(Models.Color.Id), nameof(Models.Color.Name));
                ViewBag.Categories = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name));
                ViewBag.Sizes = new SelectList(_context.Sizes, nameof(Models.Size.Id), nameof(Models.Size.Name));
                return View();
            }
            if (id is null || id != productVM.Id) BadRequest();
            Product existedproduct = _context.Products.Include(p => p.ProductImages).Include(p => p.ProductColors).Include(p => p.ProductCategories).Include(p => p.ProductSizes).FirstOrDefault(p => p.Id == id);
            if (existedproduct == null) return NotFound();
            foreach (var pc in existedproduct.ProductCategories)
            {
                if (productVM.CategoryIds.Contains(pc.CategoryId))
                {
                    productVM.CategoryIds.Remove(pc.CategoryId);
                }
                else _context.ProductCategories.Remove(pc);
            }
            foreach (var pc in existedproduct.ProductColors)
            {
                if (productVM.ColorIds.Contains(pc.ColorId))
                {
                    productVM.ColorIds.Remove(pc.ColorId);
                }
                else _context.ProductColors.Remove(pc);

            }
            foreach (var ps in existedproduct.ProductSizes)
            {
                if (productVM.SizeIds.Contains(ps.SizeId))
                {
                    productVM.SizeIds.Remove(ps.SizeId);
                }
                else _context.ProductSizes.Remove(ps);
            }
            foreach (var item in (productVM.SizeIds ?? new List<int>()))
            {
                _context.ProductSizes.Add(new ProductSize { Product = existedproduct, SizeId = item });
            }
            foreach (var item in (productVM.ColorIds ?? new List<int>()))
            {
                _context.ProductSizes.Add(new ProductSize { Product = existedproduct, SizeId = item });
            }
            foreach (var item in (productVM.CategoryIds ?? new List<int>()))
            {
                _context.ProductCategories.Add(new ProductCategory { CategoryId = item, Product = existedproduct });
            }
            existedproduct.Name = productVM.Name;
            existedproduct.SellPrice = productVM.SellPrice;
            existedproduct.CostPrice = productVM.CostPrice;
            existedproduct.Description = productVM.Description;
            existedproduct.Discount = productVM.Discount;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UpdateImage(int? id)
        {
            if(id==null) BadRequest();
            var product = _context.Products.Include(p=>p.ProductImages).FirstOrDefault(p=>p.Id==id);
            if (product == null) return NotFound();
            UpdateImageVM imageVM = new UpdateImageVM();
            imageVM.Id = product.Id;
            imageVM.ProductImages = product.ProductImages;
            return View(imageVM);
        }
        [HttpPost]
        public IActionResult UpdateImage(int? id,UpdateImageVM imageVM)
        {
            var Deleteable = imageVM.ImageIds;
            var coverImg = imageVM.CoverImage;
            var hoverImg = imageVM.HoverImage;
            var otherImg = imageVM.OtherImages;
            if (id == null) BadRequest();
            var existproduct = _context.Products.Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);
            if (existproduct == null) return NotFound();
            var images = new List<ProductImage>();
            string result = coverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("CoverImage", result);
            }
            result = hoverImg?.CheckValidate("image/", 300);
            if (result?.Length > 0)
            {
                ModelState.AddModelError("HoverImage", result);
            }
            if (otherImg != null)
            {
                foreach (IFormFile image in otherImg)
                {
                    result = image?.CheckValidate("image/", 300);
                    if (result?.Length > 0)
                    {
                        ModelState.AddModelError("OtherImages", result);
                    }
                }
            }
            if (otherImg != null)
            {
                foreach (var item in otherImg)
                {
                    images.Add(new ProductImage { ImgUrl = item.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "product")), IsCover = null, Product = existproduct });
                }
            }
            if (hoverImg != null)
            {
                var existedhover = _context.ProductImages.FirstOrDefault(p => p.IsCover == false);
                if (existedhover != null)
                {
                    _context.ProductImages.Remove(existedhover);
                    existedhover.ImgUrl.DeleteFile(_webHostEnvironment.WebRootPath, "assets/images/product");
                }
                images.Add(new ProductImage { ImgUrl = hoverImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "product")), IsCover = false, Product = existproduct });
            }
            if (coverImg != null)
            {
                var existedcover = _context.ProductImages.FirstOrDefault(p => p.IsCover == true);
                if(existedcover != null)
                {
                    _context.ProductImages.Remove(existedcover);
                    existedcover.ImgUrl.DeleteFile(_webHostEnvironment.WebRootPath, "assets/images/product");
                }
                images.Add(new ProductImage { ImgUrl = coverImg.SaveFile(Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "product")), IsCover = true, Product = existproduct });
            }
            foreach (var item in images)
            {
                existproduct.ProductImages.Add(item);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
