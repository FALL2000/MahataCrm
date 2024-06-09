using CrmMahata.Models;
using MahataCrm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MahataCrm.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<ServicePlan> Services { get; set; }
        public DbSet<MahataCrm.Models.OperatorCreateViewModel> OperatorCreateViewModel { get; set; } = default!;
        public DbSet<MahataCrm.Models.LoginViewModel> LoginViewModel { get; set; } = default!;
        public DbSet<MahataCrm.Models.OperatorViewModel> OperatorViewModel { get; set; } = default!;
    }
}
