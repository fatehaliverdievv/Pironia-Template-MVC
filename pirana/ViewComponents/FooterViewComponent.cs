using Microsoft.AspNetCore.Mvc;
using pirana.DAL;

namespace pirana.ViewComponents
{
    public class FooterViewComponent:ViewComponent
    {
        readonly AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_context.Settings.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
