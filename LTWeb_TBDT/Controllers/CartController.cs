using LTWeb_TBDT.Data;
using LTWeb_TBDT.Helpers;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LTWeb_TBDT.Controllers
{
	public class CartController : Controller
	{
        private readonly BanThietBiDienTuContext db;

        private const string CartSessionKey = "CartItems";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>();
        public CartController(BanThietBiDienTuContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {

            return View(Cart);
        }

      
        public IActionResult AddToCart(int id, int quantity = 1)

        {
            //if (!user.IsInRole("User"))
            //{
            //    // Trả về lỗi hoặc chuyển hướng đến trang không được phép
            //    return Unauthorized();
            //}
            if (quantity <= 0)
            {
                TempData["messageCart"] = "Số lượng không hợp lệ. Vui lòng chọn số lượng lớn hơn 0.";
                return RedirectToAction("Detail", "HangHoa", new { id });

            }
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(gh => gh.MaSanPham == id);
            if (item == null)
            {
                var hangHoa = db.SanPhams.SingleOrDefault(gh => gh.MaSanPham == id);
                if (hangHoa == null)
                {
                    TempData["message"] = $"Không tìm thấy mã {id} trong giỏ hàng";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaSanPham = hangHoa.MaSanPham,
                    TenSanPham = hangHoa.TenSanPham,
                    GiaBan = hangHoa.GiaBan ,
                    HinhAnh = hangHoa.HinhAnh ?? string.Empty,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
             //   item.SoLuong += quantity;
                item.SoLuong = quantity;

            }

            HttpContext.Session.Set(StaticMethod.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaSanPham == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(StaticMethod.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckOut()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/"); // chuyển đến 1 url cụ thể
            }
            return View(Cart);
        }

           
    }
}
