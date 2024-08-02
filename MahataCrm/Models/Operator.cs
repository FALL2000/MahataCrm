using CrmMahata.Models;
using Microsoft.AspNetCore.Identity;

namespace MahataCrm.Models
{

    public enum StatusOperator
    {
        Active,
        Inactive
    }
    public class Operator: IdentityUser
    {
        public String Name { get; set; }
        public StatusOperator Status { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Log> Logs { get; set; }
        public ICollection<OperatorMatchAccount> OperatorMatchAccounts { get; set; }
    }
}
