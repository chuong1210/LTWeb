using LTWeb_TBDT.Data;
using Microsoft.AspNetCore.Mvc;

namespace LTWeb_TBDT.Controllers
{
    public class HDController : Controller
    {
        private readonly BanThietBiDienTuContext db;
        private readonly ConnectHoaDon _connectHoaDon;
        private readonly ConnnectChiTietHoaDon _connectChiTietHoaDon;
        public HDController(BanThietBiDienTuContext context, ConnectHoaDon connectHoaDon, ConnnectChiTietHoaDon connectChiTietHoaDon)
        {
            db = context;

            _connectHoaDon = connectHoaDon;
            _connectChiTietHoaDon = connectChiTietHoaDon;
        }
        public IActionResult Index()
        {
            return View(db.HoaDons.ToList());
        }



     
        }
    }

