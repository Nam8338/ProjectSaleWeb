using AspNetCoreHero.ToastNotification.Abstractions;
using SmartZone.Models;
using SmartZone.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartZone.Controllers
{
    public class DonHangController : Controller
    {
        private readonly MyDbContext _context;
        public INotyfService _notyfService { get; } //Import services

        public DonHangController(MyDbContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        public Order Detail { get; set; }
        
        
        [HttpGet]
        [Route("chi-tiet-don-hang-{id}.html", Name = "Chitietdonhang")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var taikhoannID = HttpContext.Session.GetString("CustommerId");
            if (string.IsNullOrEmpty(taikhoannID))
                return RedirectToAction("Login", "Accounts");
            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == Convert.ToInt32(taikhoannID));
            if (khachhang == null)
                return NotFound();
            var donhang = _context.Orders
                .Include(x => x.TransactionStatus)
                .Include(x=>x.Customer)
                .FirstOrDefault(m => m.OrderId == id && Convert.ToInt32(taikhoannID) == m.CustomerId);
            if (donhang == null)
            {
                return NotFound();
            }
            var chitietdonhang = _context.OrderDetails
                .Include(x => x.Product).AsNoTracking()
                .Where(x => x.OrderId == id)
                .OrderBy(x => x.OrderDetailId).ToList(); // ma de kieu do thi no bi conflict
            XemDonHang donHang = new XemDonHang(); // khoi tao viewmodel, oce nay dm k thay, TA TV lung tung dau ca mat
            donHang.DonHang = donhang;
            donHang.ChiTietDonHang = chitietdonhang;
            ViewBag.donHang = donHang;
            return View(donHang);
            //return PartialView("Details", donHang);
        }

        //Hủy đơn hàng
        [HttpPost]
        [Route("api/donhang/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                var _Order = _context.Orders.AsNoTracking().Where(x => x.OrderId == id).SingleOrDefault();
                if (_Order != null)
                {
                    _Order.IsDeleted = true;
                    _context.Update(_Order);
                    _context.SaveChanges();
                }
                _notyfService.Success("Đã hủy thành công");
                return Json(new { succcess = true });
            }
            catch (Exception)
            {
                return Json(new { succcess = false });
            }
        }
    }
}
