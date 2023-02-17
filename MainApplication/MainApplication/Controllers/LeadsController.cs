using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainApplication.Data;
using MainApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace MainApplication.Controllers
{
    [Authorize]
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Leads
        public async Task<IActionResult> Index()
        {
              return View(await _context.AppLead.ToListAsync());
        }

        // GET: Leads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AppLead == null)
            {
                return NotFound();
            }

            var appLeadEntity = await _context.AppLead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appLeadEntity == null)
            {
                return NotFound();
            }

            return View(appLeadEntity);
        }

        // GET: Leads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Mobile,Email,Role")] AppLeadEntity appLeadEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appLeadEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appLeadEntity);
        }

        // GET: Leads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AppLead == null)
            {
                return NotFound();
            }

            var appLeadEntity = await _context.AppLead.FindAsync(id);
            if (appLeadEntity == null)
            {
                return NotFound();
            }
            return View(appLeadEntity);
        }

        // POST: Leads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Mobile,Email,Role")] AppLeadEntity appLeadEntity)
        {
            if (id != appLeadEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appLeadEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppLeadEntityExists(appLeadEntity.Id))
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
            return View(appLeadEntity);
        }

        // GET: Leads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AppLead == null)
            {
                return NotFound();
            }

            var appLeadEntity = await _context.AppLead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appLeadEntity == null)
            {
                return NotFound();
            }

            return View(appLeadEntity);
        }

        // POST: Leads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AppLead == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AppLead'  is null.");
            }
            var appLeadEntity = await _context.AppLead.FindAsync(id);
            if (appLeadEntity != null)
            {
                _context.AppLead.Remove(appLeadEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppLeadEntityExists(int id)
        {
          return _context.AppLead.Any(e => e.Id == id);
        }
    }
}
