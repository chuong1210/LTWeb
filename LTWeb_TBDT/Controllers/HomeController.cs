using System.Diagnostics;
using LTWeb_TBDT.Data;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? danhMucId, int? nhaSanXuatId, int? minPrice=0, int? maxPrice=1000000000, int page = 1, int pageSize = 6)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Nếu người dùng đã đăng nhập, kiểm tra vai trò

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("DashBoard", "Manager");
                }
            }
            IQueryable<LTWeb_TBDT.Data.SanPham> query = _context.SanPhams
                                         .Include(s => s.MaNhaSanXuatNavigation)
                                         .Include(s => s.MaDanhMucNavigation);

			if (danhMucId.HasValue)
			{
				query = query.Where(sp => sp.MaDanhMuc == danhMucId.Value);
				ViewBag.DanhMucId = danhMucId; // Truyền giá trị sang View
			}

			if (nhaSanXuatId.HasValue)
			{
				query = query.Where(sp => sp.MaNhaSanXuat == nhaSanXuatId.Value);
				ViewBag.NhaSanXuatId = nhaSanXuatId; // Truyền giá trị sang View
			}


            if (minPrice.HasValue)
            {
                query = query.Where(sp => sp.GiaBan >= minPrice.Value);
                ViewBag.MinPrice = minPrice;
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(sp => sp.GiaBan <= maxPrice.Value);
                ViewBag.MaxPrice = maxPrice;
            }
            // Tính tổng số sản phẩm
            int totalItems = await query.CountAsync();

            // Phân trang bằng Skip và Take
            List<LTWeb_TBDT.Data.SanPham> products = await query
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
            .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);



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
