using AspNetCoreHero.ToastNotification.Abstractions;
using SmartZone.Extension;
using SmartZone.Models;
using SmartZone.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartZone.Controllers
{
    public class ShopCartController : Controller
    {
        private readonly MyDbContext _context;
        public INotyfService _notyfService { get; } //Import services
        private readonly IConfiguration _config;

        private readonly string _clientId;
        private readonly string _sercretKey;

        public decimal TygiaUSD = 23000;

        public ShopCartController(MyDbContext context, INotyfService notyfService, IConfiguration config)
        {
            _notyfService = notyfService;
            _context = context;
            _clientId = config["PayPalSettings: ClientId"];
            _sercretKey = config["PayPalSettings: SecretKey"];
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
            for(var i = 0; i < carts.Count; i++)
            {
                if(carts[i].product.ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }

        #region Thao tác trên giỏ hàng

        // 1. Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productId, int? qty)
        {
            List<CartItem> carts = GioHang;
            try
            {
                Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productId);
                //m có lưu vào db ddauad mà lấy ra đc ._. uar, vào sessionn mà, xài viewmodel th,
                CartItem item = GioHang.SingleOrDefault(x => x.product.ProductId == productId);
                if (carts == null)
                {
                   
                    carts.Add(new CartItem
                    {
                        Qty = qty.HasValue ? qty.Value : 1,
                        product = hh
                    });
                }
                else
                {
                    int index = Exists(carts, productId);
                    if(index == -1)
                    {
                        carts.Add(new CartItem
                        {
                            Qty = qty.HasValue ? qty.Value : 1, // chưa có mặc định nó về 1
                            product = hh
                        });
                    }
                    else
                    {
                        carts[index].Qty += qty;
                    }
                }
                //if (item != null) // giỏ hàng có đồ
                //{
                //    //if (qty.HasValue)
                //    //{
                //    //    item.Qty = qty.Value; // số lượng = số lượng nhập vào
                //    //}
                //    //else
                //    //{
                //    //    item.Qty++; // Số lượng tăng 1
                //    //}
                //    item.Qty++;
                //}
                //else
                //{
                //    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productId);
                //    item = new CartItem
                //    {
                //        Qty = qty.HasValue ? qty.Value : 1,
                //        product = hh
                //    };
                //    carts.Add(item);
                //}
                //có nghĩa là m k update đc? ừ, chỉ nhập đc 1 lần duy nhất, những lần sau thêm ko update đc nữa

                GetSession.Set(HttpContext.Session, "GioHang", carts);
                //HttpContext.Session.Set<List<CartItem>>("GioHang", carts);
                _notyfService.Success("Thêm thành công vào giỏ hàng");
                return Json(new { succcess = true });
            }
            catch
            {
                return Json(new { succcess = false });
            }
        }

        // 2. Cập nhật giỏ hàng
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productId, int? qty)
        {
            List<CartItem> carts = GioHang;
            try
            {
                if (carts != null)
                {
                    //CartItem item = GioHang.SingleOrDefault(x => x.product.ProductId == productId);
                    //if (item != null && qty.HasValue) // giỏ hàng có đồ
                    //{
                    //    item.Qty = qty.Value; // số lượng = số lượng nhập vào
                    //}

                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productId);
                    int index = Exists(carts, productId);
                    if (index == -1)
                    {
                        carts.Add(new CartItem
                        {
                            Qty = qty.HasValue ? qty.Value : 1, // chưa có mặc định nó về 1
                            product = hh
                        });
                    }
                    else
                    {
                        carts[index].Qty = qty.Value;
                    }

                    GetSession.Set(HttpContext.Session, "GioHang", carts);
                    _notyfService.Success("Update thành công");
                    return Json(new { succcess = true });
                }
                return Json(new { succcess = false });
            }
            catch
            {
                return Json(new { succcess = false });
            }
        }

        // 3. xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int productId)
        {
            try
            {
                List<CartItem> carts = GioHang;
                CartItem item = carts.SingleOrDefault(a => a.product.ProductId == productId);
                if (item != null)
                {
                    carts.Remove(item);
                }
                GetSession.Set(HttpContext.Session, "GioHang", carts);
                _notyfService.Success("Xóa thành công sản phẩm khỏi giỏ hàng");
                return Json(new { succcess = true });
            }
            catch (Exception)
            {
                return Json(new { succcess = false });
            }
        }
        #endregion

        [Route("CartItem.html", Name = "GioHang")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
