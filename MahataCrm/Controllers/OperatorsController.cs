using CrmMahata.Models;
using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class OperatorsController : Controller
    {
        private readonly UserManager<Operator> _userManager;
        private readonly SignInManager<Operator> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OperatorsController(UserManager<Operator> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Operator> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
                var result = await _signInManager.PasswordSignInAsync(Logvm.Email, Logvm.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                }
            }

            return View();
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
