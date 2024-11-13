using CrmMahata.Models;
using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MahataCrm.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;

        public CustomerController(ApplicationDbContext context, UserManager<Operator> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CustomerController
        public async Task<IActionResult> Index()
        {
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            var operatorMatch = _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
            var applicationDbContext = _context.Customers.Where(r => r.AccountID == operatorMatch.BussinessId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CustomerController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,CustIdType,CustId,CustNum")] Customer Customer)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                var operatorMatch = _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
                Customer.AccountID = operatorMatch.BussinessId;
                _context.Add(Customer);
                Log log = new Log();
                log.Action = "Create";
                log.TimeAction = DateTime.Now;
                log.DateAction = DateTime.Now.Date;
                log.ActionOn = "Customer";
                log.OperatorID = CurrentUser.Id;
                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Customer", new { id = Customer.Id });
            }
            return View(Customer);
        }

        // GET: CustomerController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("Id,FirstName,LastName,CustIdType,CustId,CustNum, AccountID")] Customer Customer)
        {
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (id != Customer.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(Customer.Id))
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
                log.ActionOn = "Customer";
                log.OperatorID = CurrentUser.Id;
                _context.Add(log);
                return RedirectToAction("Details", "Customer", new { id = Customer.Id });
            }
            return View(Customer);
        }

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
