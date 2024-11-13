using CrmMahata.Models;
using MahataCrm.Data;
using MahataCrm.Models;
using MahataCrm.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using System.Diagnostics;

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class OperatorsController : Controller
    {
        private readonly UserManager<Operator> _userManager;
        private readonly SignInManager<Operator> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public OperatorsController(UserManager<Operator> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Operator> signInManager, ApplicationDbContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
        }


        [Authorize(Roles = "Super Admin")]
        // GET: OperatorsController
        public async Task<IActionResult> Index()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var AuditorRole = await _roleManager.FindByNameAsync("Auditor");

            var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole.Name);
            var Admins = adminUsers.Select(
                  u => new OperatorViewModel
                  {
                      Id = u.Id,
                      Name = u.Name,
                      Email = u.Email,
                      Status = u.Status,
                      Role = "Admin"
                  });
            var AuditorUsers = await _userManager.GetUsersInRoleAsync(AuditorRole.Name);
            var Auditors = AuditorUsers.Select(
                  u => new OperatorViewModel
                  {
                      Id = u.Id,
                      Name = u.Name,
                      Email = u.Email,
                      Status = u.Status,
                      Role = "Auditor"
                  });
            var users = Admins.Concat(Auditors).ToList();
            return View(users);
        }

        [Authorize(Roles = "Super Admin")]
        // GET: OperatorsController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            OperatorViewModel model = new OperatorViewModel();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            model.Id = user.Id;
            model.Email = user.Email;
            model.Name = user.Name;
            model.Status = user.Status;
            var Roles = await _userManager.GetRolesAsync(user);
            model.Role = Roles.FirstOrDefault();
            return View(model);
        }

        [Authorize(Roles = "Super Admin")]
        // GET: OperatorsController/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Super Admin")]
        // POST: OperatorsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperatorCreateViewModel Opvm)
        {
            if (ModelState.IsValid)
            {
                Operator Operator = new Operator();
                Operator.Email = Opvm.Email;
                Operator.Name = Opvm.Name;
                Operator.UserName = Opvm.Email;
                Operator.Status = StatusOperator.Active;
                var result = await _userManager.CreateAsync(Operator, Opvm.Password);
                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync(Opvm.Role))
                    {
                        await _userManager.AddToRoleAsync(Operator, Opvm.Role);
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                } 
            }
            
            return View();
        }

        private string CreateMessage(string code)
        {
            string cssStyles = @"
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    color: #333;
                }
                .container {
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    background: #fff;
                    border-radius: 5px;
                    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                }
                h1 {
                    color: #007bff;
                }
            </style>";
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Verifying your email address</title>
                    {cssStyles}
                </head>
                <body>
                    <div class='container'>
                        <h1>Verifying your email address</h1>
                        <p>Hello,</p>
                        <p>Thank you for registering on our platform. To confirm your email address, please enter the following verification code:</p>
                        <h2 style='background-color: #007bff; color: #fff; padding: 10px; border-radius: 5px;'>{code}</h2>
                        <p>This verification code will expire in 30 minutes.</p>
                        <p>If you have not created an account on our platform, please ignore this email.</p>
                        <p>Thank you,<br>Mahita CRM</p>
                    </div>
                </body>
                </html>";
        }

        [AllowAnonymous]
        // GET: OperatorsController/Login
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        // POST: OperatorsController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel Logvm)
        {
            if (ModelState.IsValid)
            {
                //var result = await _signInManager.PasswordSignInAsync(Logvm.Email, Logvm.Password, false, false);
                var user = await _userManager.FindByEmailAsync(Logvm.Email);
                if (user != null)
                {
                    bool isvalid = await _userManager.CheckPasswordAsync(user, Logvm.Password);
                    if (isvalid)
                    {
                        Opt opt = new Opt
                        {
                            userId = user.Id,
                            createdAt = DateTime.Now,
                            optMail = Logvm.Email,
                            optPass = Logvm.Password,
                            otp = GenerateOtpCode()
                        };

                        _context.Add(opt);
                        await _context.SaveChangesAsync();
                        try
                        {
                            await _emailSender.SendEmailAsync("axeltsotie@gmail.com", "Verification Code",
                            CreateMessage(opt.otp));
                            opt.otp = "";
                        }
                        catch
                        {
                            ModelState.AddModelError(string.Empty, "An SMTP error occurred while sending the verification code, Please check your internet connection");
                            return View();
                        }
                        
                        return View("Verif", opt);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                    }
                }

                /*if (result.Succeeded)
                {
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                    if (currentUser != null)
                    {
                       
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User not found");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                }*/
            }

            return View();
        }

        [AllowAnonymous]
        // POST: OperatorsController/Verify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verif(Opt opt)
        {
            Opt op = _context.Opts.Where(o => o.userId == opt.userId && o.otp == opt.otp).FirstOrDefault();
            if (op == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid verification code");
            }
            else
            {
                if (DateTime.Now > op.createdAt.AddMinutes(30))
                {
                    ModelState.AddModelError(string.Empty, "Verification code has expired. Please log in again");
                    return View();
                }
                else
                {
                    var result = await _signInManager.PasswordSignInAsync(op.optMail, op.optPass, false, false);
                    if (result.Succeeded)
                    {
                        _context.Opts.Remove(op);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        private string GenerateOtpCode()
        {
            // Générer un code OTP aléatoire
            Random rand = new Random();
            return rand.Next(1000, 9999).ToString();
        }

        // GET: OperatorsController/Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        // GET: OperatorsController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            OperatorEditModel model = new OperatorEditModel();
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }

            model.IdOp = user.Id;
            model.Email = user.Email;
            model.Name = user.Name;
            var Roles = await _userManager.GetRolesAsync(user);
            model.Role = Roles.FirstOrDefault();
            return View(model);
        }

        // POST: OperatorsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, OperatorEditModel Opvm)
        {

            if (id != Opvm.IdOp)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var Roles = await _userManager.GetRolesAsync(user);
                user.Email = Opvm.Email;
                user.Name = Opvm.Name;
                user.UserName = Opvm.Email;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(user, Roles.FirstOrDefault());
                    await _userManager.AddToRoleAsync(user, Opvm.Role);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }       
            }
            
            return View(Opvm);
        }

        [HttpPost]
        public async Task<IActionResult> changePassword(string oldPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                return Json(new { error = true, message = "Erron an Occured, please try again" });
            }
            user.HaveConnexion = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Json(new { error = true, message = "Erron an Occured, please try again" });
            }
            string urlDetail = Url.Action("Index", "Home");
            return Json(new { error = false, url = urlDetail });
        }

        [Authorize(Roles = "Super Admin")]
        // GET: OperatorsController/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            OperatorViewModel model = new OperatorViewModel();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            model.Id = user.Id;
            model.Email = user.Email;
            model.Name = user.Name;
            model.Status = user.Status;
            var Roles = await _userManager.GetRolesAsync(user);
            model.Role = Roles.FirstOrDefault();
            return View(model);
        }

        [Authorize(Roles = "Super Admin")]
        // POST: OperatorsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(OperatorViewModel Opvm)
        {
            var user = await _userManager.FindByIdAsync(Opvm.Id);
            if(user != null)
            {
                var result  = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }

        [Authorize(Roles = "Super Admin")]
        // GET: OperatorsController/Delete/5
        public async Task<IActionResult> Block(string? id)
        {
            OperatorViewModel model = new OperatorViewModel();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            model.Id = user.Id;
            model.Email = user.Email;
            model.Name = user.Name;
            model.Status = user.Status;
            var Roles = await _userManager.GetRolesAsync(user);
            model.Role = Roles.FirstOrDefault();
            return View(model);
        }


        [Authorize(Roles = "Super Admin")]
        // POST: OperatorsController/Delete/5
        [HttpPost, ActionName("Block")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockConfirmed(OperatorViewModel Opvm)
        {
            var user = await _userManager.FindByIdAsync(Opvm.Id);
            if (user != null)
            {
                user.Status = user.Status == 0 ? StatusOperator.Inactive : StatusOperator.Active;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }
        // GET: OperatorsController/AccessDenied
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
