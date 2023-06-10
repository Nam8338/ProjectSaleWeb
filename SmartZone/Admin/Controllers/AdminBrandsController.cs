using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartZone.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Globalization;
using System.IO;
using SmartZone.Helpper;
using PagedList.Core;   //Phân trang dữ liệu

namespace SmartZone.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBrandsController : Controller
    {
        private readonly MyDbContext _context;
        public static string image;
        public INotyfService _notyfService { get; }
        public AdminBrandsController(MyDbContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        // GET: Admin/AdminBrands
        public IActionResult Index(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            var _brand = from m in _context.Brands select m;
            //Sort
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            switch (sortOrder)
            {
                case "id_desc":
                    _brand = _brand.OrderByDescending(p => p.BrandId);
                    break;
                case "Name":
                    _brand = _brand.OrderBy(p => p.BrandName);
                    break;
                case "name_desc":
                    _brand = _brand.OrderByDescending(p => p.BrandName);
                    break;
                default:
                    _brand = _brand.OrderBy(p => p.BrandId);
                    break;
            }

            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _brand = _brand.Where(p => p.BrandId.ToString().Contains(searchStr) || p.BrandName.Contains(searchStr));
            }

            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            ViewBag.CurrentPage = pageNo;
            PagedList<Brand> models = new PagedList<Brand>(_brand, pageNo, pageSize);
            return View(models);
        }

        // GET: Admin/AdminBrands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Admin/AdminBrands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminBrands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrandId,BrandName,Logo")] Brand brand, Microsoft.AspNetCore.Http.IFormFile fLogo)
        {
            if (ModelState.IsValid)
            {
                var _brand = from m in _context.Brands select m;
                if (_brand.Any(a => a.BrandName == brand.BrandName))
                {
                    _notyfService.Error("Nhãn hàng này đã có trong Cơ sở dữ liệu!");
                    return RedirectToAction(nameof(Create));
                }
                else
                {
                    brand.BrandName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(brand.BrandName);
                    if (fLogo != null)
                    {
                        string extennsion = Path.GetExtension(fLogo.FileName);
                        image = Utilities.ToUrlFriendly(brand.BrandName) + extennsion;
                        brand.Logo = await Utilities.UploadFile(fLogo, @"brands", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(brand.Logo)) brand.Logo = "thumb-6.jpg";
                    _notyfService.Success("Thêm thành công!");
                    _context.Add(brand);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(brand);
        }

        // GET: Admin/AdminBrands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Admin/AdminBrands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrandId,BrandName,Logo")] Brand brand, Microsoft.AspNetCore.Http.IFormFile fLogo)
        {
            if (id != brand.BrandId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _brand = from m in _context.Brands select m;
                    if (_brand.Any(a => a.BrandName == brand.BrandName && a.Logo == brand.Logo))
                    {
                        _notyfService.Error("Nhãn hàng này đã có trong Cơ sở dữ liệu!");
                        return RedirectToAction(nameof(Edit));
                    }
                    else
                    {
                        brand.BrandName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(brand.BrandName);
                        if (fLogo != null)
                        {
                            string extennsion = Path.GetExtension(fLogo.FileName);
                            image = Utilities.ToUrlFriendly(brand.BrandName) + extennsion;
                            brand.Logo = await Utilities.UploadFile(fLogo, @"brands", image.ToLower());
                        }
                        if (string.IsNullOrEmpty(brand.Logo)) brand.Logo = "thumb-6.jpg";
                        _notyfService.Success("Sửa thành công!");
                        _context.Update(brand);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.BrandId))
                    {
                        _notyfService.Error("Lỗi!!!!!!!!!!!!!");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Admin/AdminBrands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Admin/AdminBrands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.BrandId == id);
        }
    }
}
