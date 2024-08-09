using Microsoft.AspNetCore.Mvc;

namespace applicationcrm.Controllers
{
    using applicationcrm.Data;
    using applicationcrm.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class ServicePlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicePlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var servicePlans = await _context.ServicePlans.Include(sp => sp.Accounts).ToListAsync();
            return View(servicePlans);
        }
    }
}
