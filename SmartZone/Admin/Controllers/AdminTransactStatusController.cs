using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Admin.Models;
using Microsoft.AspNetCore.Authorization;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTransactStatusController : Controller
    {
        private readonly MyDbContext _context;

        public AdminTransactStatusController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminTransactStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.TransactStatuses.ToListAsync());
        }

        // GET: Admin/AdminTransactStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactStatus = await _context.TransactStatuses
                .FirstOrDefaultAsync(m => m.TransactionStatusId == id);
            if (transactStatus == null)
            {
                return NotFound();
            }

            return View(transactStatus);
        }

        // GET: Admin/AdminTransactStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminTransactStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionStatusId,Status,Descriptions")] TransactStatus transactStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transactStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transactStatus);
        }

        // GET: Admin/AdminTransactStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactStatus = await _context.TransactStatuses.FindAsync(id);
            if (transactStatus == null)
            {
                return NotFound();
            }
            return View(transactStatus);
        }

        // POST: Admin/AdminTransactStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionStatusId,Status,Descriptions")] TransactStatus transactStatus)
        {
            if (id != transactStatus.TransactionStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transactStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactStatusExists(transactStatus.TransactionStatusId))
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
            return View(transactStatus);
        }

        // GET: Admin/AdminTransactStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transactStatus = await _context.TransactStatuses
                .FirstOrDefaultAsync(m => m.TransactionStatusId == id);
            if (transactStatus == null)
            {
                return NotFound();
            }

            return View(transactStatus);
        }

        // POST: Admin/AdminTransactStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactStatus = await _context.TransactStatuses.FindAsync(id);
            _context.TransactStatuses.Remove(transactStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactStatusExists(int id)
        {
            return _context.TransactStatuses.Any(e => e.TransactionStatusId == id);
        }
    }
}
