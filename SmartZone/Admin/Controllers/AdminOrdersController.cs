using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartZone.Models;
using PagedList.Core;
using EmailServices;
using Microsoft.AspNetCore.Authorization;

namespace EShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IEmailSender _emailSender;

        public AdminOrdersController(MyDbContext context, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _context = context;
        }

        #region index
        public IActionResult Index(string searchStr, int? page)
        {
            //chưa duyệt
            var ecommerceVer2Context = from m in _context.Orders.Include(o => o.Customer).Include(o => o.TransactionStatus).Where(o => o.TransactionStatusId == 1).OrderByDescending(x => x.OrderDate) select m;
            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                ecommerceVer2Context = ecommerceVer2Context.Where(p => p.OrderId.ToString().Contains(searchStr) || p.CustomerId.ToString().Contains(searchStr));
            }

            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 15;
            ViewBag.CurrentPage = pageNo;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName");
            PagedList<Order> models = new PagedList<Order>(ecommerceVer2Context, pageNo, pageSize);
            return View(models);
        }

        public IActionResult Index1(string searchStr, int? page)
        {
            //Đang chuyển
            var DeliveringProduct = from m in _context.Orders.Include(o => o.Customer).Include(o => o.TransactionStatus).Where(o => o.TransactionStatusId == 3).OrderByDescending(x => x.OrderDate) select m;
            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                DeliveringProduct = DeliveringProduct.Where(p => p.OrderId.ToString().Contains(searchStr) || p.CustomerId.ToString().Contains(searchStr));
            }

            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 15;
            ViewBag.CurrentPage = pageNo;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName");
            PagedList<Order> models = new PagedList<Order>(DeliveringProduct, pageNo, pageSize);
            return View(models);
        }

        public IActionResult Index2(string searchStr, int? page)
        {
            //Đã giao
            var DeliveredProduct = from m in _context.Orders.Include(o => o.Customer).Include(o => o.TransactionStatus).Where(o => o.TransactionStatusId == 5).OrderByDescending(x => x.OrderDate) select m;
            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                DeliveredProduct = DeliveredProduct.Where(p => p.OrderId.ToString().Contains(searchStr) || p.CustomerId.ToString().Contains(searchStr));
            }

            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 15;
            ViewBag.CurrentPage = pageNo;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName");
            PagedList<Order> models = new PagedList<Order>(DeliveredProduct, pageNo, pageSize);
            return View(models);
        }

        public IActionResult Index3(string searchStr, int? page)
        {
            //Đã trả tiền
            var paidProduct = from m in _context.Orders.Include(o => o.Customer).Include(o => o.TransactionStatus).Where(o => o.IsPaid == true).OrderByDescending(x => x.OrderDate) select m;
            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                paidProduct = paidProduct.Where(p => p.OrderId.ToString().Contains(searchStr) || p.CustomerId.ToString().Contains(searchStr));
            }

            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 15;
            ViewBag.CurrentPage = pageNo;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName");
            PagedList<Order> models = new PagedList<Order>(paidProduct, pageNo, pageSize);
            return View(models);
        }
        #endregion
        #region details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactionStatus)
                .Where(o => o.IsDeleted == false)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var chitietdohang = _context.OrderDetails
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.OrderDetailId)
                .ToList();
            ViewBag.ChiTiet = chitietdohang;
            return View(order);
        }
        #endregion
        #region Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName");
            ViewData["TransactionStatusId"] = new SelectList(_context.TransactStatuses, "TransactionStatusId", "Descriptions");
            return View();
        }

        // POST: Admin/AdminOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactionStatusId,IsDeleted,IsPaid,PaymentDate,PaymentId,Note,TotalMoney,Address,Province,District,Ward")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName", order.CustomerId);
            ViewData["TransactionStatusId"] = new SelectList(_context.TransactStatuses, "TransactionStatusId", "Descriptions", order.TransactionStatusId);
            return View(order);
        }
        #endregion
        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName", order.CustomerId);
            ViewData["TransactionStatusId"] = new SelectList(_context.TransactStatuses, "TransactionStatusId", "Descriptions", order.TransactionStatusId);
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactionStatusId,IsDeleted,IsPaid,PaymentDate,PaymentId,Note,TotalMoney,Address,Province,District,Ward")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = _context.Orders.AsNoTracking().Include(x => x.Customer).Where(x=>x.OrderId==id).FirstOrDefault();
                    if (donhang != null)
                    {
                        donhang.IsPaid = order.IsPaid;
                        donhang.IsDeleted = order.IsDeleted;
                        donhang.TransactionStatusId = order.TransactionStatusId;
                        donhang.OrderDate = order.OrderDate;
                        //Khai báo Chitietdonhang có Id = donhang.Id
                        var orderdetail = _context.OrderDetails
                                .AsNoTracking()
                                .Where(x => x.OrderId == donhang.OrderId)
                                .Include(x => x.Product)
                                .ToList();
                        //Nhóm sản phẩm theo productID sau đó tính tổng số lượng
                        foreach (var item in orderdetail)
                        {
                            var product1 = _context.OrderDetails
                                .GroupBy(x => x.ProductId)
                                .Select(g => new
                                {
                                    Key = g.Key,
                                    Total = g.Sum(x => x.Quantity)
                                })
                                .OrderByDescending(x => x.Total)
                                .ToList();
                            for (int i = 0; i < product1.Count; i++) // Chạy lặp trong danh sách sản phẩm của product1
                            {
                                if (product1[i].Total >= 5) //Đk để set nó thành BestSeller
                                {
                                    var product2 = _context.Products.Where(x => x.ProductId == product1[i].Key).FirstOrDefault(); // Kiếm nó trong Db
                                    product2.IsBestsellers = true;
                                    _context.Update(product2);
                                }
                            }
                        }
                        // Nếu trạng thái đơn hàng đã xác nhận thông tin thah toán thì chuyển qua bước vận chuyển -> trừ vào số lượng hàng trong kho
                        if (donhang.TransactionStatusId == 3)
                        {
                            string chitiet = "";
                            for (int i = 0; i < orderdetail.Count(); i++)
                            {
                                var product = _context.Products
                                    .Where(x => x.ProductId == orderdetail[i].ProductId)
                                    .FirstOrDefault();
                                product.UnitInStock = (product.UnitInStock - orderdetail[i].Quantity);
                                chitiet += "Tên sản phẩm : " + product.ProductName.ToString();
                                chitiet += " - Số lượng : " + orderdetail[i].Quantity.Value.ToString();
                                chitiet += " - Giá : " + orderdetail[i].Product.SalesPrice.Value.ToString("#,##0") + " VNĐ";
                                chitiet += " - Thành tiền : " + orderdetail[i].Total.Value.ToString("#,##0") + " VNĐ";
                                chitiet += "<br />";
                                chitiet += "Tổng thanh toán: " + donhang.TotalMoney.Value.ToString("#,##0") + " VNĐ";
                                _context.Update(product);
                            }

                            var message = new Message(new string[] { donhang.Customer.Mail }, "Xác nhận đơn hàng", chitiet);
                            _emailSender.SendEmail(message);
                        }
                        // Nếu đơn hàng đã giao thì chuyển trạng thái Delete = true;
                        if (donhang.TransactionStatusId == 5)
                        {
                            donhang.IsDeleted = true;
                        }
                    }
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustommerId", "FullName", order.CustomerId);
            ViewData["TransactionStatusId"] = new SelectList(_context.TransactStatuses, "TransactionStatusId", "Descriptions", order.TransactionStatusId);
            return View(order);
        }
        #endregion
        #region Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactionStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/AdminOrders/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
