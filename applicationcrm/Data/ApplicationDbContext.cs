using applicationcrm.Models;
using Microsoft.EntityFrameworkCore;

namespace applicationcrm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ServicePlan> ServicePlans { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
