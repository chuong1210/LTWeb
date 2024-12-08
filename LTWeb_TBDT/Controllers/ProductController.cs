using LTWeb_TBDT.Data;
using LTWeb_TBDT.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.Controllers
{
	public class ProductController : Controller
	{
		private readonly BanThietBiDienTuContext _context;

		public ProductController(BanThietBiDienTuContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}

	


	
		public async Task<IActionResult> Index1()
		{
			return View(await _context.SanPhams.ToListAsync());
		}

		// GET: Products/Details/5
		public async Task<IActionResult> Detail(int id)
		{
			var product = await _context.SanPhams
				.Include(s => s.MaNhaSanXuatNavigation)
				.Include(s => s.MaDanhMucNavigation)
				.FirstOrDefaultAsync(s => s.MaSanPham == id);

			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}
		// GET: Products/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Products/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Price,Description")] SanPham product,IFormFile formFile)
		{
			if (ModelState.IsValid)
			{
				_context.Add(product);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
		//	product.Hinh = MyUltil.UploadImage(formFile, "KhachHang");


			//if (ModelState.IsValid)
			//{
			//	try
			//	{
			//		KhachHang kh = _mapper.Map<KhachHang>(khSU);
			//		kh.RandomKey = MyUtil.GenerateRandomKey();
			//		kh.MatKhau = khSU.MatKhau.ToMd5Hash(kh.RandomKey);
			//		kh.HieuLuc = true;
			//		kh.VaiTro = 0;
			//		if (urlImage != null)
			//		{
			//		}
			//		db.Add(kh);
			//		db.SaveChanges();
			//		return RedirectToAction("Index", "HangHoa");

			//	}
			//	catch (Exception ex)
			//	{
			//		var mess = $"{ex.Message} shh";
			//	}
			//}

			return View(product);
		}

		// GET: Products/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.SanPhams.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}

		// POST: Products/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description")] SanPham product)
		{
			if (id != product.MaSanPham)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(product);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductExists(product.MaSanPham))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(product);
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.SanPhams
				.FirstOrDefaultAsync(m => m.MaSanPham == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var product = await _context.SanPhams.FindAsync(id);
			_context.SanPhams.Remove(product);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProductExists(int id)
		{
			return _context.SanPhams.Any(e => e.MaSanPham == id);
		}
	}
}
