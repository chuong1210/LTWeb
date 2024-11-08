using LTWeb_TBDT.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.ViewComponents
{
    public class DanhMucViewComponent:ViewComponent
    {
        private readonly BanThietBiDienTuContext _context;

        public DanhMucViewComponent(BanThietBiDienTuContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var danhMucList = await _context.DanhMucs.ToListAsync();
            return View(danhMucList);
        }
    }
}
