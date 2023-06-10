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

namespace SmartZone.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCategoriesController : Controller
    {
        private readonly MyDbContext _context;
        public static string image;
        public INotyfService _notyfService { get; }

        public AdminCategoriesController(MyDbContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        // GET: Admin/AdminCategories
        public IActionResult Index(string sortOrder, string currentFilter, string searchStr, int? page)
        {
            var _category = from m in _context.Categories select m;
            //Sort
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["CurrentSort"] = sortOrder;
            switch (sortOrder)
            {
                case "id_desc":
                    _category = _category.OrderByDescending(p => p.CateId);
                    break;
                case "Name":
                    _category = _category.OrderBy(p => p.CategoryName);
                    break;
                case "name_desc":
                    _category = _category.OrderByDescending(p => p.CategoryName);
                    break;
                case "title_desc":
                    _category = _category.OrderByDescending(p => p.Title);
                    break;
                case "Title":
                    _category = _category.OrderBy(p => p.Title);
                    break;
                case "Disable":
                    _category = _category.OrderBy(p => p.IsPublished);
                    break;
                case "Publish":
                    _category = _category.OrderByDescending(p => p.IsPublished);
                    break;
                default:
                    _category = _category.OrderBy(p => p.CateId);
                    break;
            }

            //Search
            ViewData["CurrentFilter"] = searchStr;
            if (!String.IsNullOrEmpty(searchStr))
            {
                _category = _category.Where(p => p.CateId.ToString().Contains(searchStr) || p.CategoryName.Contains(searchStr) || p.Descriptions.Contains(searchStr) || p.Title.Contains(searchStr));
            }

            //Paginate
            var pageNo = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            ViewBag.CurrentPage = pageNo;
            PagedList<Category> models = new PagedList<Category>(_category, pageNo, pageSize);
            return View(models);
        }

        // GET: Admin/AdminCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CateId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/AdminCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CateId,CategoryName,Descriptions,ParentId,Levels,Ordering,IsPublished,ThumbImg,Title,Alias,Cover")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumbImg)
        {
            if (ModelState.IsValid)
            {
                category.CategoryName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(category.CategoryName);
                if (fThumbImg != null)
                {
                    string extennsion = Path.GetExtension(fThumbImg.FileName);
                    image = Utilities.ToUrlFriendly(category.CategoryName) + extennsion;
                    category.ThumbImg = await Utilities.UploadFile(fThumbImg, @"categories", image.ToLower());
                }
                if (string.IsNullOrEmpty(category.ThumbImg)) category.ThumbImg = "thumb-6.jpg";
                category.Alias = Utilities.ToUrlFriendly(category.CategoryName);
                _context.Add(category);
                _notyfService.Success("Thêm thành công!");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CateId,CategoryName,Descriptions,ParentId,Levels,Ordering,IsPublished,ThumbImg,Title,Alias,Cover")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumbImg)
        {
            if (id != category.CateId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    category.CategoryName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(category.CategoryName);
                    if (fThumbImg != null)
                    {
                        string extennsion = Path.GetExtension(fThumbImg.FileName);
                        image = Utilities.ToUrlFriendly(category.CategoryName) + extennsion;
                        category.ThumbImg = await Utilities.UploadFile(fThumbImg, @"categories", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(category.ThumbImg)) category.ThumbImg = "thumb-6.jpg";
                    category.Alias = Utilities.ToUrlFriendly(category.CategoryName);
                    _context.Update(category);
                    _notyfService.Success("Sửa thành công!");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CateId))
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
            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CateId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            string imageName = image.ToLower() +
            _context.Categories.Remove(category);
            Utilities.DeleteImage(@"User", imageName);
            _notyfService.Success("Xóa thành công!");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CateId == id);
        }
    }
}
