using System.Diagnostics;
using LTWeb_TBDT.Data;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly BanThietBiDienTuContext _context;

		public HomeController(ILogger<HomeController> logger, BanThietBiDienTuContext context)
        {
            _logger = logger;
			_context = context;

		}

        public async Task<IActionResult> Index(int? danhMucId, int? nhaSanXuatId)
        {
             if (User.Identity.IsAuthenticated)
            {
                // Nếu người dùng đã đăng nhập, kiểm tra vai trò
               
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("DashBoard", "Manager"); // Điều hướng về trang quản lý nếu là admin
                }
            }
            // Lấy tất cả các sản phẩm, nếu có danh mục thì lọc theo danh mục
            List<LTWeb_TBDT.Data .SanPham> products = new List<LTWeb_TBDT.Data.SanPham>();

            if (danhMucId.HasValue)
            {
                // Lọc sản phẩm theo danh mục
                products = await _context.SanPhams.Where(sp => sp.MaDanhMuc == danhMucId.Value).ToListAsync();
                return View(products);

            }
            if (nhaSanXuatId.HasValue)
            {
                 products = await _context.SanPhams
                              .Where(p => p.MaNhaSanXuat == nhaSanXuatId)
                              .Include(p => p.MaDanhMucNavigation)
                              .Include(p => p.MaNhaSanXuatNavigation)
                              .ToListAsync();
                return View(products);

            }
          
                // Nếu không có tham số danh mục, lấy tất cả sản phẩm
                products = await _context.SanPhams
                     .Include(s => s.MaNhaSanXuatNavigation)
                     .Include(s => s.MaDanhMucNavigation)
                     .ToListAsync();


            return View(products);
        }

  //      public async Task<IActionResult> Index()
		//{
		//	List<Data.SanPham> products = await _context.SanPhams
		//			 .Include(s => s.MaNhaSanXuatNavigation)
		//			 .Include(s => s.MaDanhMucNavigation)
		//			 .ToListAsync();


		//	return View(products);
		//}
		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
