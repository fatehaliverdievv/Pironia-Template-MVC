using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pirana.DAL;

namespace pirana.ViewComponents
{
    public class QuickViewViewComponent:ViewComponent
    {
        readonly AppDbContext _context;

        public QuickViewViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
                var product = _context.Products.
                Include(p => p.ProductColors).ThenInclude(p => p.Color).Include(p => p.ProductSizes).
                ThenInclude(ps => ps.Size).Include(p => p.ProductImages)
                .Where(p => p.IsDeleted == false).FirstOrDefault(p => p.Id == id);
                return View(product);
        }
    }
}
