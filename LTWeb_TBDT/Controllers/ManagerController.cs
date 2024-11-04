using Microsoft.AspNetCore.Mvc;

namespace LTWeb_TBDT.Controllers
{
	public class ManagerController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
