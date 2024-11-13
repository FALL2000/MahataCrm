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
        public DbSet<Souscription> Souscriptions { get; set; }
        public DbSet<OperatorMatchAccount> OperatorMatchs { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Opt> Opts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Profil> Profils { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<MahataCrm.Models.OperatorCreateViewModel> OperatorCreateViewModel { get; set; } = default!;
        public DbSet<MahataCrm.Models.LoginViewModel> LoginViewModel { get; set; } = default!;
        public DbSet<MahataCrm.Models.OperatorViewModel> OperatorViewModel { get; set; } = default!;
    }
}
