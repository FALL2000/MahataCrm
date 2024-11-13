using CrmMahata.Models;
using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace MahataCrm.Controllers
{
    public class ProfilsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfilsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: ProfilsController
        public async Task<IActionResult> Index()
        {
            return View(await _context.Profils.ToListAsync());
        }

        // GET: ProfilsController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profil = await _context.Profils
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profil == null)
            {
                return NotFound();
            }

            return View(profil);
        }

        // GET: ProfilsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProfilsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Id,Name,isCreateAccount,isUpdateAccount,isDeleteAccount,isBlockAccount,isViewAccount,isCreateServicePlan,isUpdateServicePlan,isDeleteServicePlan,isAssignServicePlan,isViewServicePlan")] Profil profil)
        {
            if (ModelState.IsValid)
            {
                var existingProfil = _context.Profils.FirstOrDefault(p => p.Name == profil.Name);
                if(existingProfil != null)
                {
                    ModelState.AddModelError(string.Empty, "This name of profile already exists");
                }
                else
                {
                    _context.Add(profil);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }   
            }
            return View(profil);
        }

        // GET: ProfilsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProfilsController/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, [Bind("Id,Name,isCreateAccount,isUpdateAccount,isDeleteAccount,isBlockAccount,isViewAccount,isCreateServicePlan,isUpdateServicePlan,isDeleteServicePlan,isAssignServicePlan,isViewServicePlan")] Profil profil)
        {
            if (id != profil.Id)
            {
                return Json(new { error = true, message="Profile Not Found" });
            }
            if(profil.Name == null)
            {
                return Json(new { error = true, message = "Name field is required" });
            }
            else
            {
                var existingProfil = _context.Profils.FirstOrDefault(p => p.Name == profil.Name && p.Id != id);
                if (existingProfil != null)
                {
                    return Json(new { error = true, message = "This name of profile already exists" });
                }
                else
                {
                    _context.Update(profil);
                    await _context.SaveChangesAsync();
                    string urlDetail = Url.Action("Details", "Profils", new { id = profil.Id });
                    return Json(new { error = false, url = urlDetail });
                }
            }
        }

        // GET: ProfilsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProfilsController/Delete/5
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
    }
}
