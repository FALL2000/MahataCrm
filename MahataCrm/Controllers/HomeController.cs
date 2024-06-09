using MahataCrm.Data;
using MahataCrm.Models;
using MahataCrm.Models.ClasseUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MahataCrm.Controllers
{
    [Authorize(Policy = "RequireLoggedIn")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Operator> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<Operator> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var AccountCounts = _context.Accounts
            .GroupBy(a => a.Status)
            .Select(g => new AccountByStatusCount
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToList();
            var roles = _roleManager.Roles.ToList();
            var OperatorCounts = roles
            .Select(r => new OperatorByCountType
            {
                TypeOperator = r.Name,
                Count = _userManager.GetUsersInRoleAsync(r.Name).Result.Count
            })
            .ToList();
            ViewData["Account"] = AccountCounts;
            ViewData["Operator"] = OperatorCounts;

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
