using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmMahata.Models;
using MahataCrm.Data;
using Microsoft.AspNetCore.Authorization;

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class ServicePlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicePlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServicePlans
        public async Task<IActionResult> Index()
        {
            return View(await _context.Services.ToListAsync());
        }

        // GET: ServicePlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePlan = await _context.Services
                .Include(s => s.Accounts).FirstOrDefaultAsync(m => m.Id == id);
            if (servicePlan == null)
            {
                return NotFound();
            }

            return View(servicePlan);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: ServicePlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServicePlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Duration,Price")] ServicePlan servicePlan)
        {
            //if (ModelState.IsValid)
            //{
            _context.Add(servicePlan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            // }
            //return View(servicePlan);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: ServicePlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePlan = await _context.Services.FindAsync(id);
            if (servicePlan == null)
            {
                return NotFound();
            }
            return View(servicePlan);
        }

        // POST: ServicePlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Duration,Price")] ServicePlan servicePlan)
        {
            if (id != servicePlan.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                _context.Update(servicePlan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicePlanExists(servicePlan.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            //}
            //return View(servicePlan);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: ServicePlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePlan = await _context.Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (servicePlan == null)
            {
                return NotFound();
            }

            return View(servicePlan);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // POST: ServicePlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicePlan = await _context.Services.FindAsync(id);
            if (servicePlan != null)
            {
                _context.Services.Remove(servicePlan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicePlanExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
