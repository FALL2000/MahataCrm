using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmMahata.Models;
using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;

        public AccountsController(ApplicationDbContext context, UserManager<Operator> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Accounts.Include(a => a.ServicePlan);
            return View(await myDbContext.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.ServicePlan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: Accounts/Create
        public IActionResult Create()
        {
            ViewData["ServicePlanID"] = new SelectList(_context.Services, "Id", "Id");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdAcc,BusinessName,Address,Phone,Email,WebSite,ServicePlanID")] Account account)
        {
            //if (ModelState.IsValid)
            //{
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if(CurrentUser != null)
            {
                account.OperatorID = CurrentUser.Id;
            }
            account.Status = StatusType.Active;
            _context.Add(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //}
            //ViewData["ServicePlanID"] = new SelectList(_context.Services, "Id", "Id", account.ServicePlanID);
            //return View(account);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["ServicePlanID"] = new SelectList(_context.Services, "Id", "Id", account.ServicePlanID);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdAcc,BusinessName,Address,Phone,Email,WebSite,Status,ServicePlanID")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            // if (ModelState.IsValid)
            //{
            try
            {
                _context.Update(account);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.Id))
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
            //ViewData["ServicePlanID"] = new SelectList(_context.Services, "Id", "Id", account.ServicePlanID);
            //return View(account);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.ServicePlan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: Accounts/Block/5
        public async Task<IActionResult> Block(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.ServicePlan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // POST: Accounts/Block/5
        [HttpPost, ActionName("Block")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.Status = account.Status == 0 ? StatusType.Inactive : StatusType.Active;
                try
                {
                    _context.Update(account);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Super Admin, Admin")]
        public async Task<IActionResult> AssignSP(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.ServicePlan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["ServicePlanID"] = new SelectList(_context.Services, "Id", "Name");
            return View(account);
        }


        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost, ActionName("AssignSP")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToAccount(int id, int ServicePlanID)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.ServicePlanID = ServicePlanID;
                try
                {
                    _context.Update(account);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
