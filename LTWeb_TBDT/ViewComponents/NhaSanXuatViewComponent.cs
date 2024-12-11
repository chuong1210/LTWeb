using LTWeb_TBDT.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.ViewComponents
{
    public class NhaSanXuatViewComponent:ViewComponent
    {
        private readonly BanThietBiDienTuContext _context;

        public NhaSanXuatViewComponent(BanThietBiDienTuContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var danhMucList = await _context.NhaSanXuats.ToListAsync();
            return View(danhMucList);
        }
    }
}
