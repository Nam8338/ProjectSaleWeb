using AspNetCoreHero.ToastNotification.Abstractions;
using SmartZone.Extension;
using SmartZone.Models;
using SmartZone.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartZone.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Login")]
    public class LoginController : Controller
    {
        private readonly MyDbContext _context;
        public INotyfService _notyfService { get; } //Import services
        public static string image;

        public LoginController(MyDbContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("AdLogin.html", Name = "AdDangNhap")]
        public async Task<IActionResult> AdLogin(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // phải include thằng role vào thì em mới lấy dc du lieu tu bang role
                    var CTM = _context.Accounts.Include(x=>x.Role).AsNoTracking().SingleOrDefault(x => x.Username.Trim() == model.UserName);
                    if (CTM == null)
                    {
                        _notyfService.Error("Thông tin đăng nhập không chính xác");
                        return Redirect("/Admin/Login/Index?ReturnUrl=%2Fadmin%2Fdashboard%2Findex");
                    }
                    string pass = (model.Password + CTM.Randomkey.Trim()).PassToMD5(); //pass nhập vô

                    //Kiểm tra pass có giống vs Password ko
                    if (CTM.Password != pass)
                    {
                        _notyfService.Error("Thông tin đăng nhập không chính xác"); // so sánh pass
                        return View(CTM);
                    }

                    //Kiểm tra Acc có bị Disable không
                    if (CTM.IsActived == false)
                    {
                        _notyfService.Warning("Tài khoản này đã bị khóa, vui lòng liên hệ Admin");
                        return Redirect("/Admin/Login/Index?ReturnUrl=%2Fadmin%2Fdashboard%2Findex");
                    }

                    //Lưu luôn Session đỡ phải Login lại
                    //Lưu Session cho CustomerId
                    HttpContext.Session.SetString("UserId", CTM.UserId.ToString());
                    var AccID = HttpContext.Session.GetString("UserId");
                    //Identity
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, CTM.FullName),
                            new Claim("UserId", CTM.UserId.ToString()),
                            new Claim(ClaimTypes.Role, CTM.Role.RoleName)
                        };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Custom("Đăng nhập thành công!", 5, "#EAB14E", "fas fa-crown");
                    return RedirectToAction("Index", "Home", new { Area = "Admin" }); // sau khi tài khoản mật khẩu đúng các kiểu r thì get từ session ra xong direct về index của Home admin

                    //đợi t x
                }
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return Redirect("/Admin/Login/Index?ReturnUrl=%2Fadmin%2Fdashboard%2Findex");
            }
            catch
            {
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return Redirect("/Admin/Login/Index?ReturnUrl=%2Fadmin%2Fdashboard%2Findex");
            }
        }

        //SignOut
        [HttpGet]
        [Route("Adlogout", Name = "AdDangXuat")]
        public IActionResult Logout()
        {
            var AccID = HttpContext.Session.GetString("UserId");
            var _User = _context.Accounts.Where(x => x.UserId == Convert.ToInt32(AccID)).FirstOrDefault();
            try
            {
                if (_User != null)
                {
                    _User.LastLogin = DateTime.Now;
                    _context.Update(_User);
                    _context.SaveChanges();
                }
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("UserId");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
