using AspNetCoreHero.ToastNotification.Abstractions;
using SmartZone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace SmartZone.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminRolesController : Controller
    {
        private readonly MyDbContext _context;
        public INotyfService _notyfService { get; }

        public AdminRolesController(MyDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminRoles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // GET: Admin/AdminRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Admin/AdminRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,RoleName,Descriptions")] Role role)
        {
            if (ModelState.IsValid)
            {
                var _Roles = from m in _context.Roles select m;
                if(_Roles.Any(a=>a.RoleName == role.RoleName))
                {
                    _notyfService.Error("Trùng tên quyền");
                    return RedirectToAction(nameof(Create));
                }
                else
                {
                    _context.Add(role);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Tạo mới thành công");
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(role);
        }

        // GET: Admin/AdminRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Admin/AdminRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,RoleName,Descriptions")] Role role)
        {
            if (id != role.RoleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _Roles = from m in _context.Roles select m;
                    if (_Roles.Any(a => a.RoleName == role.RoleName))
                    {
                        _notyfService.Error("Trùng tên quyền");
                        return RedirectToAction(nameof(Create));
                    }
                    else
                    {
                        _context.Update(role);
                        await _context.SaveChangesAsync();
                        _notyfService.Success("Cập nhật thành công!");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.RoleId))
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
            return View(role);
        }

        // GET: Admin/AdminRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/AdminRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công!");
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
