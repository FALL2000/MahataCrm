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
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using MahataCrm.Service;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IWhatsappService _whatsappService;

        public AccountsController(ApplicationDbContext context, UserManager<Operator> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IWhatsappService whatsappService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _whatsappService = whatsappService;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
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
            var souscriptions = _context.Souscriptions.Where(s => s.AccountID == id).Include(s => s.ServicePlan).ToList();
            if (account == null)
            {
                return NotFound();
            }
            ViewData["souscriptions"] = souscriptions;
            return View(account);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        // GET: Accounts/Create
        public IActionResult Create()
        {
            ViewData["ServicePlanID"] = new SelectList(_context.Services, "Id", "Id");
            return View();
        }

        private string CreateMessage(string name, string email, string password)
        {
            return "<body>\r\n   " +
                " <p>Hello "+name +",</p>\r\n\r\n   " +
                " <p>We are delighted to welcome you to Mahita CRM! Your account has been successfully created.</p>\r\n\r\n    <p>Please login with the following credentials:</p>\r\n    " +
                "<ul>\r\n        <li><strong>Login:</strong>"+ email + "</li>\r\n        <li><strong>Password:</strong>" 
                + password + "</li>\r\n    " +
                "</ul>\r\n\r\n    <p>Explore our platform and discover our features :</p>\r\n    " +
                " <p>We remain at your disposal for any questions or assistance.</p>\r\n\r\n   " +
                " <p>Thank you for being part of our community!</p>\r\n\r\n    <p>Best regards,,<br>Mahita CRM</p>\r\n</body>\r\n</html>";
        }


        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BusinessName,Address,Phone,Email,WebSite,ServicePlanID,Tin,Certkey,RegId,Vrn,Serial,Uin,TaxOffice,Password")] Account account)
        {

            //if (ModelState.IsValid)
            //{
            var existingCertkey = _context.Accounts.FirstOrDefault(a => a.Certkey == account.Certkey);
            if (existingCertkey != null)
            {
                ModelState.AddModelError(string.Empty, "This value of Certkey already exists");
                return View(account);
            }
            var existingTin = _context.Accounts.FirstOrDefault(a => a.Tin == account.Tin);
            if (existingTin != null)
            {
                ModelState.AddModelError(string.Empty, "This value of Tin already exists");
                return View(account);
            }
            var existingPhone = _context.Accounts.FirstOrDefault(a => a.Phone == account.Phone);
            if (existingPhone != null)
            {
                ModelState.AddModelError(string.Empty, "This value of Phone already exists");
                return View(account);
            }
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if(CurrentUser != null)
            {
                account.OperatorID = CurrentUser.Id;
            }
            account.Status = StatusType.Active;
            account.CreatedAt = DateTime.Now.Date;
            _context.Add(account);
            Operator Operator = new Operator();
            Operator.Email = account.Email;
            Operator.Name = account.BusinessName;
            Operator.UserName = account.Email;
            Operator.Status = StatusOperator.Active;
            var result = await _userManager.CreateAsync(Operator, account.Password);
            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("BUSSINESS"))
                {
                    await _userManager.AddToRoleAsync(Operator, "BUSSINESS");
                }
                OperatorMatchAccount operatorMatchAccount = new OperatorMatchAccount();
                operatorMatchAccount.OperatorID = Operator.Id;
                operatorMatchAccount.BussinessId = account.Id;
                Log log = new Log();
                log.Action = "Create";
                log.TimeAction = DateTime.Now;
                log.DateAction = DateTime.Now.Date;
                log.ActionOn = "Account";
                log.OperatorID = CurrentUser.Id;
                _context.Add(operatorMatchAccount);
                _context.Add(log);
                await _context.SaveChangesAsync();

                await _emailSender.SendEmailAsync("axeltsotie@gmail.com", "Account Created",
                            CreateMessage(account.BusinessName, account.Email, account.Password));
                _whatsappService.EnvoyerNotificationWhatsApp(23793700371, "Account created");
                TempData["SuccessMessage"] = "L'enregistrement a été effectué avec succès.";
                return RedirectToAction("Details", "Accounts", new { id = account.Id });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(account);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,BusinessName,Address,Phone,Email,WebSite,Status,ServicePlanID,Tin,Certkey,RegId,Vrn,Serial,Uin,TaxOffice,Password")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            // if (ModelState.IsValid)
            //{
            try
            {
                var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (CurrentUser != null)
                {
                    account.OperatorID = CurrentUser.Id;
                }
                account.UpdatedAt = DateTime.Now.Date;
                _context.Update(account);
                Log log = new Log();
                log.Action = "Edit";
                log.TimeAction = DateTime.Now;
                log.DateAction = DateTime.Now.Date;
                log.ActionOn = "Account";
                log.OperatorID = CurrentUser.Id;
                _context.Add(log);
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
            return RedirectToAction("Details", "Accounts", new { id = account.Id });
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
            var accRep = await _context.Receipts
                .FirstOrDefaultAsync(m => m.AccountID == id && m.isPost);
            if (accRep != null)
            {
                ViewBag.isDelete = false;
            }
            else
            {
                ViewBag.isDelete = true;
            }
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
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
            Log log = new Log();
            log.Action = "Delete";
            log.TimeAction = DateTime.Now;
            log.DateAction = DateTime.Now.Date;
            log.ActionOn = "Account";
            log.OperatorID = CurrentUser.Id;
            _context.Add(log);

            await _context.SaveChangesAsync();
            await _emailSender.SendEmailAsync("axeltsotie@gmail.com", "Account Deleded",
                            $"Hello ,'{account.BusinessName}' , Your account deleded successfully");
            _whatsappService.EnvoyerNotificationWhatsApp(23755612059, "Account Deleded");
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
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (account != null)
            {
                account.Status = account.Status == 0 ? StatusType.Inactive : StatusType.Active;
                account.UpdatedAt = DateTime.Now.Date;
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
            Log log = new Log();
            log.Action = "Edit";
            log.TimeAction = DateTime.Now;
            log.DateAction = DateTime.Now.Date;
            log.ActionOn = "Account";
            log.OperatorID = CurrentUser.Id;
            _context.Add(log);

            await _context.SaveChangesAsync();
            await _emailSender.SendEmailAsync("axeltsotie@gmail.com", "Account Desactivated",
                            $"Hello ,'{account.BusinessName}' , Your account Desactivated successfully");
            return RedirectToAction("Details", "Accounts", new { id = account.Id });
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
            var accountSub = _context.Souscriptions.Where(s => s.AccountID == account.Id).OrderByDescending(s => s.DateSouscription).Include(s => s.ServicePlan).FirstOrDefault();
            if(accountSub != null) {
                DateTime EndDate = accountSub.DateSouscription.AddDays(accountSub.ServicePlan.Duration);
                if (DateTime.Today <= EndDate)
                {
                    ViewData["isActif"] = true;
                }
                else
                {
                    ViewData["isActif"] = false;
                }
            }
            else
            {
                ViewData["isActif"] = false;
            }
            
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
                Souscription subcribe = new Souscription();
                subcribe.DateSouscription = DateTime.Now.Date;
                subcribe.ServicePlanID = ServicePlanID;
                subcribe.AccountID = account.Id;
                _context.Add(subcribe);
            }

            await _context.SaveChangesAsync();
            await _emailSender.SendEmailAsync("axeltsotie@gmail.com", "Assign service plan",
                            $"Hello ,'{account.BusinessName}' , Your Have a new service plan");
            return RedirectToAction("AssignSP", "Accounts", new { id = account.Id });
        }

        // GET: Accounts/search
        public IActionResult searchIn()
        { 
          return View("search");
        }

        // POST: Accounts/search
        public async Task<IActionResult> search(SearchAccountModel search)
        {
            List<Account> accounts = new List<Account>();
            if (search.FromC != null && search.ToC != null)
            {
                accounts = _context.Accounts
                        .Where(t => t.CreatedAt >= search.FromC && t.CreatedAt <= search.ToC)
                        .ToList();
                return View("Index", accounts);
            }
            if (search.FromU != null && search.ToU != null)
            {
                accounts = _context.Accounts
                        .Where(t =>  t.UpdatedAt >= search.FromU && t.UpdatedAt <= search.ToU)
                        .ToList();
                return View("Index", accounts);
            }

            return View("Index", accounts);
        }


        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
