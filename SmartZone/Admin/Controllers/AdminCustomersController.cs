using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartZone.Models;
using PagedList.Core;
using SmartZone.Helpper;
using System.IO;
using System.Globalization;
using AspNetCoreHero.ToastNotification.Abstractions;
using SmartZone.Extension;
using Microsoft.AspNetCore.Authorization;

namespace SmartZone.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCustomersController : Controller
    {
        private readonly MyDbContext _context;
        public static string image;
        public INotyfService _notyfService { get; }
        public AdminCustomersController(MyDbContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        // GET: Admin/AdminCustomers
        public IActionResult Index(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            //Sort
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentSort"] = sortOrder;
            var _customer = from m in _context.Customers select m;
            switch (sortOrder)
            {
                case "name_desc":
                    _customer = _customer.OrderByDescending(x => x.FullName);
                    break;
                case "Date":
                    _customer = _customer.OrderBy(x => x.BirthDay);
                    break;
                case "date_desc":
                    _customer = _customer.OrderByDescending(x => x.BirthDay);
                    break;
                default:
                    _customer = _customer.OrderBy(s => s.FullName);
                    break;
            }

            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _customer = _customer.Where(s => s.FullName.Contains(searchStr) || s.CustommerId.ToString().Contains(searchStr) || s.Mail.Contains(searchStr) || s.Phone.Contains(searchStr) || s.Province.Contains(searchStr)|| s.Ward.Contains(searchStr) || s.District.Contains(searchStr));
            }

            //Khai báo để phân trang
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5; /*Utilities.PAGE_SIZE;*/
            PagedList<Customer> models = new PagedList<Customer>(_customer, pageNo, pageSize);

            ViewBag.CurrentPage = pageNo;
            return View(models);
        }


        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustommerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustommerId,Username,Password,FullName,BirthDay,Avatar,Address,Mail,Phone,Province,District,Ward,CreateDate,LastLogin,IsActived,Randomkey,CartId")] Customer customer, Microsoft.AspNetCore.Http.IFormFile fAvatar)
        {
            if (ModelState.IsValid)
            {
                customer.FullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(customer.FullName);
                string RandomKey = Utilities.GetRandomKey();
                if (fAvatar != null)
                {
                    string extennsion = Path.GetExtension(fAvatar.FileName);
                    image = Utilities.ToUrlFriendly(customer.FullName) + extennsion;
                    customer.Avatar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
                }
                if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "avatar.png";
                customer.CreateDate = DateTime.Now;
                customer.LastLogin = DateTime.Now;
                customer.Password = (customer.Password + RandomKey.Trim()).PassToMD5();
                customer.Randomkey = RandomKey;
                _context.Add(customer);
                _notyfService.Success("Thêm thành công!");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Edit(int id, [Bind("CustommerId,Username,Password,FullName,BirthDay,Avatar,Address,Mail,Phone,Province,District,Ward,CreateDate,LastLogin,IsActived,Randomkey,CartId")] Customer customer, Microsoft.AspNetCore.Http.IFormFile fAvatar)
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
                    var _custommer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == customer.CustommerId);
                    string newpass = (customer.Password.Trim() + _custommer.Randomkey.Trim()).PassToMD5();
                    customer.Password = newpass;
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

        // GET: Admin/AdminCustomers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustommerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/AdminCustomers/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            string imageName = image.ToLower();
            Utilities.DeleteImage(@"User", imageName);
            _context.Customers.Remove(customer);
            _notyfService.Success("Xóa thành công!");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustommerId == id);
        }
    }
}
