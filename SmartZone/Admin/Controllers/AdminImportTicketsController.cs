using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartZone.Models;

namespace Admin.Controllers
{
    public class AdminImportTicketsController : Controller
    {
        private readonly MyDbContext _context;

        public AdminImportTicketsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminImportTickets
        public async Task<IActionResult> Index()
        {
            var ecommerceVer2Context = _context.ImportTickets.Include(i => i.User);
            return View(await ecommerceVer2Context.ToListAsync());
        }

        // GET: Admin/AdminImportTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importTicket = await _context.ImportTickets
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (importTicket == null)
            {
                return NotFound();
            }

            return View(importTicket);
        }

        // GET: Admin/AdminImportTickets/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "FullName");
            return View();
        }

        // POST: Admin/AdminImportTickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,ImportDate,TotalMoney,UserId")] ImportTicket importTicket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(importTicket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "FullName", importTicket.UserId);
            return View(importTicket);
        }

        // GET: Admin/AdminImportTickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importTicket = await _context.ImportTickets.FindAsync(id);
            if (importTicket == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "FullName", importTicket.UserId);
            return View(importTicket);
        }

        // POST: Admin/AdminImportTickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,ImportDate,TotalMoney,UserId")] ImportTicket importTicket)
        {
            if (id != importTicket.TicketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(importTicket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImportTicketExists(importTicket.TicketId))
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
            ViewData["UserId"] = new SelectList(_context.Accounts, "UserId", "FullName", importTicket.UserId);
            return View(importTicket);
        }

        // GET: Admin/AdminImportTickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importTicket = await _context.ImportTickets
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (importTicket == null)
            {
                return NotFound();
            }

            return View(importTicket);
        }

        // POST: Admin/AdminImportTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var importTicket = await _context.ImportTickets.FindAsync(id);
            _context.ImportTickets.Remove(importTicket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImportTicketExists(int id)
        {
            return _context.ImportTickets.Any(e => e.TicketId == id);
        }
    }
}
