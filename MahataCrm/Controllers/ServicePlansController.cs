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
using MahataCrm.Models;
using Microsoft.AspNetCore.Identity;

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class ServicePlansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;

        public ServicePlansController(ApplicationDbContext context, UserManager<Operator> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            _context.Add(servicePlan);
            Log log = new Log();
            log.Action = "Create";
            log.TimeAction = DateTime.Now;
            log.DateAction = DateTime.Now.Date;
            log.ActionOn = "Service Plan";
            log.OperatorID = CurrentUser.Id;
            _context.Add(log);
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
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
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
            Log log = new Log();
            log.Action = "Edit";
            log.TimeAction = DateTime.Now;
            log.DateAction = DateTime.Now.Date;
            log.ActionOn = "Service Plan";
            log.OperatorID = CurrentUser.Id;
            _context.Add(log);
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
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (servicePlan != null)
            {
                _context.Services.Remove(servicePlan);
            }
            Log log = new Log();
            log.Action = "Delete";
            log.TimeAction = DateTime.Now;
            log.DateAction = DateTime.Now.Date;
            log.ActionOn = "Service Plan";
            log.OperatorID = CurrentUser.Id;
            _context.Add(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicePlanExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
