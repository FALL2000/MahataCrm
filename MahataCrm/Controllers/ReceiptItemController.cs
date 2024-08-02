using MahataCrm.Data;
using MahataCrm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MahataCrm.Controllers
{
    public class ReceiptItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReceiptItemController(ApplicationDbContext context) {
            _context = context;
        }
        // GET: ReceiptItemController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReceiptItemController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReceiptItemController/Create
       /* public ActionResult Create()
        {
            return View();
        }*/

        // POST: ReceiptItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> Create([FromBody] ICollection<ReceiptItem> ReceiptItems)
        {
            foreach (var item in ReceiptItems)
            {  
                _context.ReceiptItems.Add(item);
            }

            await _context.SaveChangesAsync();
            return true;
            /*try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }*/
        }

        // GET: ReceiptItemController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReceiptItemController/Edit/5
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

        // GET: ReceiptItemController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReceiptItemController/Delete/5
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
