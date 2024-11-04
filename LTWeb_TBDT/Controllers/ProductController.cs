using Microsoft.AspNetCore.Mvc;

namespace LTWeb_TBDT.Controllers
{
	public class ProductController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Detail()
		{
			return View();
		}
	}
}
