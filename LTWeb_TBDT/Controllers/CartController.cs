using Microsoft.AspNetCore.Mvc;

namespace LTWeb_TBDT.Controllers
{
	public class CartController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Checkout()
		{
			return View();
		}
	}
}
