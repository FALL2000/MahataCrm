using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MahataCrm.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;

        public ProductController(ApplicationDbContext context, UserManager<Operator> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: ProductController
        public async Task<IActionResult> Index()
        {
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            var operatorMatch = _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
            var applicationDbContext = _context.Products.Where(r => r.AccountID == operatorMatch.BussinessId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Description, Price")] Product Product)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                var operatorMatch = _context.OperatorMatchs.FirstOrDefault(r => r.OperatorID == CurrentUser.Id);
                Product.AccountID = operatorMatch.BussinessId;
                _context.Add(Product);
                Log log = new Log();
                log.Action = "Create";
                log.TimeAction = DateTime.Now;
                log.DateAction = DateTime.Now.Date;
                log.ActionOn = "Product";
                log.OperatorID = CurrentUser.Id;
                _context.Add(log);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Product", new { id = Product.Id });
            }
            return View(Product);
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(int id)
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
            return View(product);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("Id,Description,Price, AccountID")] Product Product)
        {
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (id != Product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(Product.Id))
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
                log.ActionOn = "Product";
                log.OperatorID = CurrentUser.Id;
                _context.Add(log);
                return RedirectToAction("Details", "Product", new { id = Product.Id });
            }
            return View(Product);
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
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

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
