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

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(ApplicationDbContext context, UserManager<Operator> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            var myDbContext = _context.Accounts.Include(a => a.ServicePlan).Where(a => a.OperatorID == CurrentUser.Id);
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
        public async Task<IActionResult> Create([Bind("Id,BusinessName,Address,Phone,Email,WebSite,ServicePlanID,Tin,Certkey,RegId,Vrn,Serial,Uin,TaxOffice,Password")] Account account)
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
                return RedirectToAction(nameof(Index));
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
            log.Action = "Delete";
            log.TimeAction = DateTime.Now;
            log.DateAction = DateTime.Now.Date;
            log.ActionOn = "Account";
            log.OperatorID = CurrentUser.Id;
            _context.Add(log);

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
        public IActionResult Signature()
        { 
          return View();
        }
            public async Task<IActionResult> UploadCertificate(SignatureModel signModel)
        {

            // Get the data submitted from the form
            string distinguishedName = signModel.DistinguishedName;
            string serialNumber = signModel.SerialNumber;
            string signature = signModel.Signature;

            // Reconstruct the data to verify the signature
           // string dataToVerify = $"{distinguishedName}|{serialNumber}";

            // Load the public key from the certificate to verify the signature
           // X509Certificate2 certificate = new X509Certificate2(/* path to your server's public certificate */);
           /* RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToVerify);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            bool isValid = rsa.VerifyData(dataBytes, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);*/



           /* if (isValid)
            {
                // Signature is valid, proceed with your logic
                Console.WriteLine("Signature Verified: " + isValid);
            }
            else
            {
                // Invalid signature, handle the error
            }*/
            return View();
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
