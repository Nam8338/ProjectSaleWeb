using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartZone.Models;
using PagedList.Core;
using System.Globalization;
using System.IO;
using SmartZone.Helpper;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Admin.Controllers
{
    public class AdminProductsController : Controller
    {
        private readonly MyDbContext _context;
        public static string image;
        public INotyfService _notyfService { get; }

        public AdminProductsController(MyDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminProducts
        public IActionResult Index(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            var _product = from m in _context.Products.Include(p => p.Brand).Include(p => p.Cate) select m;
            //Sort
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["UnitSortParm"] = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "id_desc":
                    _product = _product.OrderByDescending(p => p.ProductId);
                    break;
                case "Date":
                    _product = _product.OrderBy(p => p.DateCreated);
                    break;
                case "date_desc":
                    _product = _product.OrderByDescending(p => p.DateCreated);
                    break;
                case "unit_desc":
                    _product = _product.OrderByDescending(p => p.UnitInStock);
                    break;
                case "Unit":
                    _product = _product.OrderBy(p => p.UnitInStock);
                    break;
                default:
                    _product = _product.OrderBy(p => p.ProductId);
                    break;
            }
            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _product = _product.Where(p => p.ProductName.Contains(searchStr) || p.ProductId.ToString().Contains(searchStr));
            }
            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            PagedList<Product> models = new PagedList<Product>(_product, pageNo, pageSize);
            ViewBag.CurrentPage = pageNo;

            return View(models);
        }

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Cate)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CategoryName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Descriptions,CateId,Price,Discount,ThumbImg,Video,DateCreated,DateModified,IsBestsellers,Homeflag,IsActived,Tag,Title,Alias,UnitInStock,BrandId,SalesPrice")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumbImg)
        {
            if (ModelState.IsValid)
            {
                product.ProductName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(product.ProductName);
                if (fThumbImg != null)
                {
                    string extennsion = Path.GetExtension(fThumbImg.FileName);
                    image = Utilities.ToUrlFriendly(product.ProductName) + extennsion;
                    product.ThumbImg = await Utilities.UploadFile(fThumbImg, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.ThumbImg)) product.ThumbImg = "thumb-6.jpg";
                product.Alias = Utilities.ToUrlFriendly(product.ProductName);
                product.DateCreated = DateTime.Now;
                product.DateModified = DateTime.Now;
                product.SalesPrice = product.Price.Value - (product.Price.Value * (product.Discount.Value) / 100);
                if (product.Discount == null)
                {
                    product.Discount = 0;
                }
                _context.Add(product);
                _notyfService.Success("Thêm thành công!");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CategoryName", product.CateId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CategoryName", product.CateId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Descriptions,CateId,Price,Discount,ThumbImg,Video,DateCreated,DateModified,IsBestsellers,Homeflag,IsActived,Tag,Title,Alias,UnitInStock,BrandId,SalesPrice")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumbImg)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(product.ProductName);
                    if (fThumbImg != null)
                    {
                        string extennsion = Path.GetExtension(fThumbImg.FileName);
                        image = Utilities.ToUrlFriendly(product.ProductName) + extennsion;
                        product.ThumbImg = await Utilities.UploadFile(fThumbImg, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.ThumbImg)) product.ThumbImg = "thumb-6.jpg";
                    product.Alias = Utilities.ToUrlFriendly(product.ProductName);
                    product.DateModified = DateTime.Now;
                    product.SalesPrice = product.Price.Value - (product.Price.Value * (product.Discount.Value) / 100);
                    if (product.Discount == null)
                    {
                        product.Discount = 0;
                    }
                    _context.Update(product);
                    _notyfService.Success("Sửa thành công!");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CategoryName", product.CateId);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Cate)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }


        public IActionResult SPGanHet(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            var _product = from m in _context.Products.Include(p => p.Brand).Include(p => p.Cate).Where(p=>p.UnitInStock < 5) select m;
            //Sort
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["UnitSortParm"] = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "id_desc":
                    _product = _product.OrderByDescending(p => p.ProductId);
                    break;
                case "Date":
                    _product = _product.OrderBy(p => p.DateCreated);
                    break;
                case "date_desc":
                    _product = _product.OrderByDescending(p => p.DateCreated);
                    break;
                case "unit_desc":
                    _product = _product.OrderByDescending(p => p.UnitInStock);
                    break;
                case "Unit":
                    _product = _product.OrderBy(p => p.UnitInStock);
                    break;
                default:
                    _product = _product.OrderBy(p => p.ProductId);
                    break;
            }

            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _product = _product.Where(p => p.ProductName.Contains(searchStr) || p.ProductId.ToString().Contains(searchStr));
            }
            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            PagedList<Product> models = new PagedList<Product>(_product, pageNo, pageSize);
            ViewBag.CurrentPage = pageNo;

            return View(models);
        }

        public IActionResult BestSeller(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            var _product = from m in _context.Products.Include(p => p.Brand).Include(p => p.Cate).Where(x=>x.IsBestsellers == true) select m;
            //Sort
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["UnitSortParm"] = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "id_desc":
                    _product = _product.OrderByDescending(p => p.ProductId);
                    break;
                case "Date":
                    _product = _product.OrderBy(p => p.DateCreated);
                    break;
                case "date_desc":
                    _product = _product.OrderByDescending(p => p.DateCreated);
                    break;
                case "unit_desc":
                    _product = _product.OrderByDescending(p => p.UnitInStock);
                    break;
                case "Unit":
                    _product = _product.OrderBy(p => p.UnitInStock);
                    break;
                default:
                    _product = _product.OrderBy(p => p.ProductId);
                    break;
            }
            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _product = _product.Where(p => p.ProductName.Contains(searchStr) || p.ProductId.ToString().Contains(searchStr));
            }
            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            PagedList<Product> models = new PagedList<Product>(_product, pageNo, pageSize);
            ViewBag.CurrentPage = pageNo;

            return View(models);
        }
    }
}
