using Microsoft.AspNetCore.Mvc;
using SmartZone.Extension;
using SmartZone.ViewModel;

namespace SmartZone.Controllers.Component
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var carts = HttpContext.Session.Get<List<CartItem>>("GioHang");
            int soluong = 0;
            if (carts != null)
            {
                soluong = carts.Count();
            }
            return View(carts);
        }
    }
}
