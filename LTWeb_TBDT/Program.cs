using LTWeb_TBDT.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<VnpayPayment>(); // Đăng ký VnpayPayment

//builder.Services.AddScoped<VnpayPayment>(provider =>
//{
//    var config = provider.GetRequiredService<IConfiguration>();
//    var tmnCode = config["Vnpay:TmnCode"];
//    var hashSecret = config["Vnpay:HashSecret"];
//    var baseUrl = config["Vnpay:BaseUrl"];
//    var callbackUrl = config["Vnpay:CallbackUrl"];

//    return new VnpayPayment();
//});
// ??ng kư DbContext
builder.Services.AddDbContext<BanThietBiDienTuContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ??ng kư các service ConnectHoaDon và ConnectChiTietHoaDon
builder.Services.AddScoped<ConnectHoaDon>();  // ??ng kư ConnectHoaDon
builder.Services.AddScoped<ConnnectChiTietHoaDon>();  // ??ng kư ConnectChiTietHoaDon

// ??ng kư Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/User/Login";
		options.AccessDeniedPath = "/User/AccessDenied";
	});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    // options.AddPolicy("VipOnly", policy => policy.RequireRole("UserVip1", "UserVip2")); gom nhóm
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kiểm tra vai trò của người dùng và điều hướng đến trang tương ứng
//app.Use(async (context, next) =>
//{
//    var user = context.User;
//    var path = context.Request.Path.Value.ToLower();

//    if (user.Identity.IsAuthenticated)
//    {
//        // Nếu người dùng là Admin và chưa ở trang Dashboard, chuyển hướng tới Dashboard
//        if (user.IsInRole("Admin") && !path.Contains("/manager/dashboard"))
//        {
//            context.Response.Redirect("/Manager/Dashboard");
//            return;
//        }
//        // Nếu người dùng không phải Admin và chưa ở trang Home, chuyển hướng tới Home
//        else if (!user.IsInRole("Admin") && !path.Contains("/home/index"))
//        {
//            context.Response.Redirect("/Home/Index");
//            return;
//        }
//    }

//    await next(); // Tiến hành xử lý yêu cầu tiếp theo nếu không có chuyển hướng
//});


app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//Scaffold - DbContext "Server=USER\MSSQLSERVER01;Database=BanThietBiDienTu;User ID=sa;Password=101204;
//Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer - OutputDir Data - Force
