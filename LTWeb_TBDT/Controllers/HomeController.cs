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

		public async Task<IActionResult> Index()
		{
			List<Data.SanPham> products = await _context.SanPhams
					 .Include(s => s.MaNhaSanXuatNavigation)
					 .Include(s => s.MaDanhMucNavigation)
					 .ToListAsync();

			return View(products);
		}
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
