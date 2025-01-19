using CrmMahata.Models;
using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MahataCrm.Controllers
{
    public class TemplateEmailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Operator> _userManager;

        public TemplateEmailController(ApplicationDbContext context, UserManager<Operator> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: TemplateEmailController
        public ActionResult Index()
        {
            return View();
        }

        // POST: TemplateEmail/GetTemplate
        [HttpPost]
        public ActionResult GetTemplate (string libelle)
        {
            var tmp = _context.TemplateEmails.FirstOrDefault(a => a.TypeEmail == libelle);
            if (tmp != null)
            {
                return Json(new { result = tmp });
            }
            else
            {
                return Json(new { text = "No data" });
            }
        }

        // GET: TemplateEmailController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TemplateEmail/PutTemplate
        [HttpPost]
        public async Task<ActionResult> PutTemplate(TemplateEmail tmpEmail, string libelle)
        {
            var tmp = _context.TemplateEmails.FirstOrDefault(a => a.TypeEmail == libelle);
            if(tmp == null)
            {
                _context.Add(tmpEmail);
            }
            else
            {
                tmp.Object = tmpEmail.Object;
                tmp.Footer = tmpEmail.Footer;
                tmp.Header = tmpEmail.Header;
                _context.Update(tmp);
            }
            await _context.SaveChangesAsync();
            return Json(new { text = "success" });
        }

        // GET: TemplateEmailController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TemplateEmailController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: TemplateEmailController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}
