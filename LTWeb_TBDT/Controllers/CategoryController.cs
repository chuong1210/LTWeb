using Microsoft.AspNetCore.Mvc;

namespace LTWeb_TBDT.Controllers
{
	public class CategoryController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
