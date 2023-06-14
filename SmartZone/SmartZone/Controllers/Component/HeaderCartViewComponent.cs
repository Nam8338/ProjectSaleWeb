using Microsoft.AspNetCore.Mvc;
using SmartZone.Extension;
using SmartZone.ViewModel;

namespace SmartZone.Controllers.Component
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var carts = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(carts);
        }
    }
}
