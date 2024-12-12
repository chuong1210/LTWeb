using LTWeb_TBDT.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly BanThietBiDienTuContext db;
        private readonly ConnectHoaDon _connectHoaDon;
        private readonly ConnnectChiTietHoaDon _connectChiTietHoaDon;

        public HoaDonController(BanThietBiDienTuContext context, ConnectHoaDon connectHoaDon, ConnnectChiTietHoaDon connectChiTietHoaDon)
        {
            db = context;
            _connectHoaDon = connectHoaDon;
            _connectChiTietHoaDon = connectChiTietHoaDon;

        }
        public IActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                var hoadon = db.HoaDons.Where(hd => hd.MaHoaDon == id).FirstOrDefault();
                hoadon.TrangThai = "Đã duyệt";
                db.Update(hoadon);
                db.SaveChanges();

            }
            return View(db.HoaDons.ToList());
        }
        public IActionResult DuyetDon(int id)
        {
            var hoadon = db.HoaDons.Where(hd => hd.MaHoaDon == id).FirstOrDefault();
            hoadon.TrangThai = "Đã duyệt";
            db.Update(hoadon);
            db.SaveChangesAsync();
            return RedirectToAction("Index", db.HoaDons.ToList());
        }
        public IActionResult Detail(int id)
        {

            {
                var order = db.HoaDons
                                          .Include(hd => hd.ChiTietHoaDons)
                                          .ThenInclude(cthd => cthd.MaSanPhamNavigation)
                                          .FirstOrDefault(hd => hd.MaHoaDon == id);

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }


        }
    }
}
