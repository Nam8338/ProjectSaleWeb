using AspNetCoreHero.ToastNotification.Abstractions;
using SmartZone.Extension;
using SmartZone.Helpper;
using SmartZone.Models;
using SmartZone.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using PayPal.Core;
using System.Threading.Tasks;
using PayPal.v1.Payments;
using BraintreeHttp;

namespace SmartZone.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MyDbContext _context;
        public INotyfService _notyfService { get; } //Import services
        private readonly string _clientId;
        private readonly string _sercretKey;

        public decimal TygiaUSD = 23000;

        public CheckoutController(MyDbContext context, INotyfService notyfService, IConfiguration config)
        {
            _notyfService = notyfService;
            _context = context;
            _clientId = "AbtX5Y3y3Ubpu8szhQt7TRTataPTU2AJDoXWAZYB3Yd146xLgk1uCcU2AGLuRYVUtXQ6qc8xrNWA9D6h";
            _sercretKey = "ELbkNgUwtUw3-FqspkjsWQPJ3EjmbOPUkCapg57keQylBMRwp0JUh40jJTFnV6WjYsqQ8FgWYFL3k8si";
        }

        #region Khởi tạo giỏ hàng
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang"); // Lấy từ trong Session
                if (gh == default(List<CartItem>)) //có thì thôi
                {
                    gh = new List<CartItem>(); // ko thì tạo
                    HttpContext.Session.Set<List<CartItem>>("GioHang", gh);
                }
                return gh;
            }
        }
        #endregion

        private int Exists(List<CartItem> carts, int id)
        {
            for (var i = 0; i < carts.Count; i++)
            {
                if (carts[i].product.ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }

        #region //ThanhToanThuong
        [Route("Checkout.html", Name = "Checkout")]
        public IActionResult Index(string url = null)
        {
            List<CartItem> carts = GioHang;
            var CustomerId = HttpContext.Session.GetString("CustommerId");
            OrderViewModel model = new OrderViewModel();
            if (CustomerId != null)
            {
                var _customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == Convert.ToInt32(CustomerId));
                model.CustomerId = _customer.CustommerId;
                model.FullName = _customer.FullName;
                model.Email = _customer.Mail;
                model.Phone = _customer.Phone;
                model.Address = _customer.Address;
                model.Province = _customer.Province;
                model.District = _customer.District;
                model.Ward = _customer.Ward;
            }
            ViewBag.GioHang = carts;
            return View(model);
        }

        [HttpPost]
        [Route("Checkout.html", Name = "Checkout")]
        public IActionResult Index(OrderViewModel model)
        {
            //lấy ra để xử lý
            List<CartItem> carts = GioHang;
            var _CustomerId = HttpContext.Session.GetString("CustommerId");
            OrderViewModel order = new OrderViewModel();
            if (_CustomerId != null)
            {
                var _customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == Convert.ToInt32(_CustomerId));
                order.CustomerId = _customer.CustommerId;
                order.FullName = _customer.FullName;
                order.Email = _customer.Mail;
                order.Phone = _customer.Phone;
                order.Note = model.Note;
                order.Address = _customer.Address;

                model.Province = _customer.Province;
                model.District = _customer.District;
                model.Ward = _customer.Ward;

                _context.Update(_customer);
                _context.SaveChanges();
            }
            try
            {
                //Khởi tạo đơn hàng
                Models.Order donhang = new Models.Order();
                donhang.CustomerId = order.CustomerId;
                donhang.Address = order.Address;
                donhang.Province = order.Province = model.Province;
                donhang.District = order.District = model.District;
                donhang.Ward = order.Ward = model.Ward;

                donhang.OrderDate = DateTime.Now;
                donhang.TransactionStatusId = 1;
                donhang.IsDeleted = false;
                donhang.IsPaid = false;
                donhang.Note = Utilities.StripHTML(order.Note);
                donhang.TotalMoney = Convert.ToDecimal(carts.Sum(x => x.TotalMoney));
                _context.Add(donhang);
                _context.SaveChanges();

                // tạo danh sách đơn hàng

                foreach (var item in carts)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = donhang.OrderId;
                    orderDetail.ProductId = item.product.ProductId;
                    orderDetail.Quantity = item.Qty;
                    orderDetail.Price = item.product.SalesPrice;
                    orderDetail.Total = (orderDetail.Price * orderDetail.Quantity);
                    orderDetail.CreateDate = DateTime.Now;
                    _context.Add(orderDetail);
                }
                _context.SaveChanges();
                HttpContext.Session.Remove("GioHang");
                _notyfService.Success("Đơn đặt thành công. Đang chờ xét duyệt");
                return RedirectToAction("Success", "Checkout");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region //Thanh toán Paypal
        public async Task<IActionResult> PaypalCheckoutAsync()
        {
            var environment = new SandboxEnvironment(_clientId, _sercretKey);
            var client = new PayPalHttpClient(environment);

            #region Create Paypal Order
            var itemList = new ItemList()
            {
                Items = new List<Item>()
            };
            var total = Math.Round(GioHang.Sum(p => p.TotalMoney.Value)/ TygiaUSD, 2);
            foreach (var item in GioHang)
            {
                itemList.Items.Add(new Item()
                {
                    Name = item.product.ProductName,
                    Currency = "USD",
                    Price = Math.Round(item.product.SalesPrice.Value / TygiaUSD, 2).ToString(),
                    Quantity = item.Qty.ToString(),
                    Sku = "sku",
                    Tax = "0"
                });
            }
            #endregion

            var PaypalId = DateTime.Now.Ticks;
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = total.ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax = "0",
                                Shipping = "0",
                                Subtotal = total.ToString()
                            }
                        },
                        ItemList = itemList,
                        Description = $"Invoice #{PaypalId}",
                        InvoiceNumber = PaypalId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = $"{hostname}/Checkout/CheckoutFail",
                    ReturnUrl = $"{hostname}/Checkout/CheckoutSuccess"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };
            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);

            try
            {
                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                var links = result.Links.GetEnumerator();
                string paypalRedirectUrl = null;
                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment  
                        paypalRedirectUrl = lnk.Href;
                    }
                }

                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                //Process when Checkout with Paypal fails
                return Redirect("/Checkout/CheckoutFail");
            }
        }

        public IActionResult CheckoutFail()
        {
            return View();
        }

        public IActionResult CheckoutSuccess(OrderViewModel model)
        {
            List<CartItem> carts = GioHang;
            var _CustomerId = HttpContext.Session.GetString("CustommerId");
            OrderViewModel order = new OrderViewModel();
            if (_CustomerId != null)
            {
                var _customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == Convert.ToInt32(_CustomerId));
                order.CustomerId = _customer.CustommerId;
                order.FullName = _customer.FullName;
                order.Email = _customer.Mail;
                order.Phone = _customer.Phone;
                order.Address = _customer.Address;

                model.Province = _customer.Province;
                model.District = _customer.District;
                model.Ward = _customer.Ward;

                _context.Update(_customer);
                _context.SaveChanges();
            }
            try
            {
                //Khởi tạo đơn hàng
                Models.Order donhang = new Models.Order();
                donhang.CustomerId = order.CustomerId;
                donhang.Address = order.Address;
                donhang.Province = order.Province = model.Province;
                donhang.District = order.District = model.District;
                donhang.Ward = order.Ward = model.Ward;

                donhang.OrderDate = DateTime.Now;
                donhang.TransactionStatusId = 1;
                donhang.IsDeleted = false;
                donhang.IsPaid = true;
                donhang.Note = Utilities.StripHTML(model.Note);
                donhang.TotalMoney = Convert.ToDecimal(carts.Sum(x => x.TotalMoney));
                _context.Add(donhang);
                _context.SaveChanges();

                // tạo danh sách đơn hàng

                foreach (var item in carts)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = donhang.OrderId;
                    orderDetail.ProductId = item.product.ProductId;
                    orderDetail.Quantity = item.Qty;
                    orderDetail.Price = item.product.SalesPrice;
                    orderDetail.Total = (orderDetail.Price * orderDetail.Quantity);
                    orderDetail.CreateDate = DateTime.Now;
                    _context.Add(orderDetail);
                }
                _context.SaveChanges();
                HttpContext.Session.Remove("GioHang");
                _notyfService.Success("Đơn đặt thành công. Đang chờ xét duyệt");
            }
            catch (Exception)
            {

                throw;
            }
            ViewBag.GioHang = carts;
            return RedirectToAction("Success", "Checkout");
        }
        #endregion

        [Route("Order-Status.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var _CustomerId = HttpContext.Session.GetString("CustommerId");
                if (string.IsNullOrEmpty(_CustomerId))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/Order-Success.html" });
                }
                var _customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == Convert.ToInt32(_CustomerId));


                var _order = _context.Orders
                    .Where(x => x.CustomerId == Convert.ToInt32(_CustomerId))
                    .OrderByDescending(x => x.OrderDate).FirstOrDefault();

                OrderSuccessViewModel orderSuccess = new OrderSuccessViewModel();
                orderSuccess.FullName = _customer.FullName;
                orderSuccess.DonHangID = _order.OrderId;
                orderSuccess.NgayDat = _order.OrderDate;
                orderSuccess.Phone = _customer.Phone;
                orderSuccess.Address = _customer.Address;
                orderSuccess.TinhThanh = _order.Province;
                orderSuccess.QuanHuyen = _order.District;
                orderSuccess.PhuongXa = _order.Ward;
                return View(orderSuccess);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
