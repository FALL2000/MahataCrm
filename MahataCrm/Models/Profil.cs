using MahataCrm.Models.ClasseUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace MahataCrm.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Profil
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isCreateAccount { get; set; }
        public bool isUpdateAccount { get; set;}
        public bool isDeleteAccount { get; set; }
        public bool isBlockAccount { get; set; } 
        public bool isViewAccount { get; set;}
        public bool isCreateServicePlan { get; set; }
        public bool isUpdateServicePlan { get;set; }
        public bool isDeleteServicePlan { get;set;}
        public bool isAssignServicePlan { get; set; }
        public bool isViewServicePlan { get; set; }
        public ICollection<Operator>? Operators { get; set; }
    }
}
