using Microsoft.AspNetCore.Mvc;

namespace LTWeb_TBDT.Controllers
{
	public class UserController:Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			return View();
		}
	}
}
