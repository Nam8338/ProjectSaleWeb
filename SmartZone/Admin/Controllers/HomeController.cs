using SmartZone;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartZone.Models;
using System.Data;
using System.Diagnostics;


namespace Admin.Controllers
{
    [Authorize(Roles = "Admin,Staff")]      //chỉ người dùng có vai trò là "Admin" hoặc "Staff" mới được phép truy cập vào phương thức hoặc lớp chứa nó. (Điều này cho phép cả quản trị viên và nhân viên có quyền truy cập)
    [Area("Admin")] //cho biết rằng phương thức hoặc lớp này thuộc về khu vực "Admin" quản lý
    [Route("Admin/Dashboard")]  //xác định rằng phương thức hoặc lớp này sẽ được truy cập thông qua đường dẫn URL "Admin/Dashboard". Khi một yêu cầu được gửi đến đường dẫn này, phương thức hoặc lớp chứa chú thích này sẽ được kích hoạt.
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;
        public HomeController(MyDbContext context)
        {
            _context = context;
        }

        [Route("Index")]
        public ActionResult Index()
        {
            //Thống kê tổng số khách hàng
            var lstCus = _context.Customers.Count();
            ViewBag.sumCus = lstCus;

            //Thống kê tổng số đơn hàng
            var lstOrder = _context.Orders.Count();
            ViewBag.sumOrder = lstOrder;

            //Thống kê tổng số sản phẩm
            var lstProduct = _context.Products.Count();
            ViewBag.sumProduct = lstProduct;

            //Thống kê những đơn hàng chưa duyệt
            var lstUnapprovedOrder = _context.Orders.Where(x => x.TransactionStatusId == 1).Count();
            ViewBag.lstUnapprovedOrder = lstUnapprovedOrder;

            //Thống kê doanh thu tháng này
            var mRevenue = _context.Orders.Where(x => x.TransactionStatusId == 3 && x.OrderDate.Month == DateTime.Now.Month).Sum(x => x.TotalMoney).Value.ToString("#,##0");
            ViewBag.mRevenue = mRevenue;

            //Thống kê doanh thu năm này
            var yRevenue = _context.Orders.Where(x => x.TransactionStatusId == 3 && x.OrderDate.Year == DateTime.Now.Year).Sum(x => x.TotalMoney).Value.ToString("#,##0");
            ViewBag.yRevenue = yRevenue;

            //Thống kê sản phẩm bán chạy
            var bSellProduct = _context.Products.Where(x => x.IsBestsellers == true).Count();
            ViewBag.bSellProduct = bSellProduct;
            //Thống kê Lợi nhuận
            return View();
        }
    }
}