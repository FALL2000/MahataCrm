using MahataCrm.Data;
using MahataCrm.Models;
using MahataCrm.Models.ClasseUtils;
using MahataCrm.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

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

        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;
            var rolesCurrentUser = ((ClaimsIdentity)user.Identity).FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var CurrentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (CurrentUser != null)
            {
                ViewBag.haveConnexion = CurrentUser.HaveConnexion; 
            }
            if (rolesCurrentUser.Contains("Bussiness"))
            {
                int CurrentYear = DateTime.Now.Year;
                var resultats = _context.Receipts
                    .Where(r => r.isPost && r.RctDate.Year == CurrentYear)
                    .GroupBy(r => new { r.RctDate.Month })
                    .Select(g => new ReceiptPostByMonth
                    {
                        Month = g.Key.Month,
                        ReceiptNumber = g.Count(),
                        Amount = g.Sum(r => r.TotalTaxIncl)
                    })
                    .ToList();
                ViewData["Receipts"] = resultats;
                try
                {
                    var tokenResponse = await APIService.GetAccessToken("demo", "rehema");
                    if (bool.Parse(tokenResponse["isError"]?.ToString()) == false)
                    {
                        var token = tokenResponse["accessToken"]?.ToString();
                        var jsonResponse = await APIService.GetDashboardData(token);
                        if (jsonResponse["isError"] == null)
                        {
                            var dashboard = jsonResponse["dashboard"];
                            if (dashboard != null)
                            {
                                ViewBag.totalRct = dashboard["total_rct"];
                                ViewBag.totalAmount = dashboard["total_amount"];
                                ViewBag.dateRct = dashboard["date"];
                                ViewBag.totalMonthAmount = dashboard["total_month_amount"];
                                ViewBag.isNone = false;
                            }
                            else
                            {
                                ViewBag.isNone = true;
                            }
                            ViewBag.isError = false;
                            ViewBag.isConError = false;
                            return View();
                        }
                        else
                        {
                            ViewBag.isConError = false;
                            ViewBag.isError = true;
                            ViewBag.ErrorStatus = jsonResponse["ErrorStatus"];
                            ViewBag.Error = jsonResponse["Error"];
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.isConError = false;
                        ViewBag.isError = true;
                        ViewBag.ErrorStatus = tokenResponse["ErrorStatus"];
                        ViewBag.Error = tokenResponse["Error"];
                        return View();
                    }
                }
                catch (HttpRequestException ex)
                {
                    ViewBag.isConError = false;
                    ViewBag.Error = "Connection Error: we were unable to connect";
                    return View();
                }

            }
            else
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

            
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
