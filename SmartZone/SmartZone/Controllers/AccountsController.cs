using AspNetCoreHero.ToastNotification.Abstractions;
using SmartZone.Extension;
using SmartZone.Helpper;
using SmartZone.Models;
using SmartZone.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SmartZone.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly MyDbContext _context;
        public INotyfService _notyfService { get; } //Import services
        public static string image;

        public AccountsController(MyDbContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        [Route("Index.html", Name = "TrangChu")]
        public IActionResult Index()
        {
            return View();
        }

        //MyAccount
        [Route("MyAccount.html", Name = "TaiKhoanCuaToi")]
        public IActionResult MyAccount()
        {
            var AccID = HttpContext.Session.GetString("CustommerId");
            if (AccID != null)
            {
                var _custommer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == Convert.ToInt32(AccID));
                if (_custommer != null)
                {
                    var lstOrder = _context.Orders
                        .AsNoTracking()
                        .Include(x => x.TransactionStatus)
                        .Where(x => x.CustomerId == _custommer.CustommerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag._lstOrder = lstOrder;
                    return View(_custommer);
                }
            }
            return RedirectToAction("Login", "Accounts");
        }

        //SignOut
        [HttpGet]
        [Route("logout", Name = "DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustommerId");
            return RedirectToAction("Index", "Home");
        }

        #region //Change Pass

        [HttpGet]
        [Route("ChangePassword.html", Name = "DoiMatKhau")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ChangePassword.html", Name = "DoiMatKhau")]
        public IActionResult ChangePassword(ChangePasswordViewModel changePassword)
        {
            try
            {
                var AccID = HttpContext.Session.GetString("CustommerId");
                if (AccID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var _customer = _context.Customers.Find(Convert.ToInt32(AccID));
                    if (_customer == null) return RedirectToAction("Login", "Accounts");
                    var pass = (changePassword.CurrentPassword.Trim() + _customer.Randomkey.Trim()).PassToMD5();
                    if (pass == _customer.Password)
                    {
                        string newpass = (changePassword.Password.Trim() + _customer.Randomkey.Trim()).PassToMD5();
                        _customer.Password = newpass;
                        _context.Update(_customer);
                        _notyfService.Success("Update thành công");
                        _context.SaveChanges();
                        return RedirectToAction("MyAccount", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Error("Update 0 thành công");
                return RedirectToAction("MyAccount", "Accounts");
            }
            _notyfService.Error("Update 0 thành công");
            return RedirectToAction("MyAccount", "Accounts");
        }
        #endregion

        #region  //Edit
        // GET: Admin/AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var AccID = HttpContext.Session.GetString("CustommerId"); // So sánh ID trong Session với ID của tài khoản đang được Edit
            if (id != Convert.ToInt32(AccID))
            {
                return RedirectToAction("Logout", "Accounts");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/AdminCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustommerId,Username,Password,FullName,BirthDay,Avatar,Address,Mail,Phone,Province,District,Ward,CreateDate,LastLogin,IsActived,Randomkey")] Customer customer, Microsoft.AspNetCore.Http.IFormFile fAvatar)
        {
            if (id != customer.CustommerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(customer.FullName);
                    if (fAvatar != null)
                    {
                        string extennsion = Path.GetExtension(fAvatar.FileName);
                        image = Utilities.ToUrlFriendly(customer.FullName) + extennsion;
                        customer.Avatar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "avatar.png";
                    customer.LastLogin = DateTime.Now;
                    _notyfService.Success("Sửa thành công!");
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustommerId))
                    {
                        _notyfService.Error("Lỗi!!!!!!!!!!!!");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        private bool CustomerExists(int UserId)
        {
            return _context.Customers.Any(e => e.CustommerId == UserId);
        }
        #endregion

        #region //Validate
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var _Customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone);
                if (_Customer != null)
                {
                    return Json(data: "Số : " + Phone + " này đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Mail)
        {
            try
            {
                var _Customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Mail.ToLower() == Mail.ToLower());
                if (_Customer != null)
                {
                    return Json(data: "Email : " + Mail + " này đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateUserName(string Username)
        {
            try
            {
                var _Customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Username.ToLower() == Username.ToLower());
                if (_Customer != null)
                {
                    return Json(data: "Tên tài khoản : " + Username + " này đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }
        #endregion

        #region //Đăng ký
        [HttpGet]
        [AllowAnonymous]
        [Route("Register.html", Name = "DangKy")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Register.html", Name = "DangKy")]
        public async Task<IActionResult> Register(RegisterViewModel Taikhoan, Microsoft.AspNetCore.Http.IFormFile fAvatar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string RandomKey = Utilities.GetRandomKey();
                    if (fAvatar != null)
                    {
                        string extennsion = Path.GetExtension(fAvatar.FileName);
                        image = Utilities.ToUrlFriendly(Taikhoan.FullName) + extennsion;
                        Taikhoan.Avatar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(Taikhoan.Avatar)) Taikhoan.Avatar = "avatar.png";
                    Customer Acc = new Customer
                    {
                        FullName = Taikhoan.FullName,
                        Username = Taikhoan.Username,
                        BirthDay = Taikhoan.BirthDay,
                        Phone = Taikhoan.Phone.Trim().ToLower(),
                        Mail = Taikhoan.Mail.Trim().ToLower(),
                        Address = Taikhoan.Address,
                        Province = Taikhoan.Province,
                        District = Taikhoan.District,
                        Ward = Taikhoan.Ward,
                        Password = (Taikhoan.Password + RandomKey.Trim()).PassToMD5(),
                        IsActived = true,
                        Randomkey = RandomKey,
                        CreateDate = DateTime.Now,
                        Avatar = Taikhoan.Avatar
                    };
                    try
                    {
                        _context.Add(Acc);
                        _notyfService.Success("Tạo thành công!");
                        await _context.SaveChangesAsync();

                        //Lưu luôn Session đỡ phải Login lại
                        //Lưu Session cho CustomerId
                        HttpContext.Session.SetString("CustommerId", Acc.CustommerId.ToString());
                        var AccID = HttpContext.Session.GetString("CustommerId");
                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, Acc.FullName),
                            new Claim("CustommerId", Acc.CustommerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Index", "Home");
                    }
                    catch
                    {
                        _notyfService.Error("Lỗi khi tạo tài khoản! Chuyển hướng về trang Đăng Ký");
                        return RedirectToAction("Register", "Accounts");
                    }
                }
                else
                {
                    return View(Taikhoan);
                }
            }
            catch
            {

                return View(Taikhoan);
            }
        }
        #endregion

        #region //Đăng nhập
        [AllowAnonymous]
        [Route("Login.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var AccID = HttpContext.Session.GetString("CustommerId");
            var test = User.Identity.IsAuthenticated;
            if (AccID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var CTM = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Username.Trim() == model.UserName);
                    if (CTM == null)
                    {
                        _notyfService.Error("Thông tin đăng nhập không chính xác");
                        return RedirectToAction("Login", "Accounts");
                    }
                    string pass = (model.Password + CTM.Randomkey.Trim()).PassToMD5();

                    //Kiểm tra pass có giống vs Password ko
                    if (CTM.Password != pass)
                    {
                        _notyfService.Error("Thông tin đăng nhập không chính xác");
                        return View(CTM);
                    }

                    //Kiểm tra Acc có bị Disable không
                    if (CTM.IsActived == false)
                    {
                        _notyfService.Warning("Tài khoản của bạn đang bị khóa, vui lòng liên hệ Admin!");
                        return RedirectToAction("Login", "Accounts");
                    }

                    //Lưu luôn Session đỡ phải Login lại
                    //Lưu Session cho CustomerId
                    HttpContext.Session.SetString("CustommerId", CTM.CustommerId.ToString());
                    var AccID = HttpContext.Session.GetString("CustommerId");
                    //Identity
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, CTM.FullName),
                            new Claim("CustommerId", CTM.CustommerId.ToString())
                        };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                    _notyfService.Custom("Đăng nhập thành công!", 5, "#EAB14E", "fas fa-crown");
                    return RedirectToAction("Index", "Home");
                }
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return RedirectToAction("Register", "Accounts");
            }
            catch
            {
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return RedirectToAction("Register", "Accounts");
            }
        }
        #endregion
    }
}
