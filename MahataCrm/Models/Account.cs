
using MahataCrm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace CrmMahata.Models
{

    public enum StatusType
    {
        Active,
        Inactive
    }

    [Index(nameof(IdAcc), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Account
    {
        public int Id { get; set; }
        public string IdAcc { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public StatusType Status { get; set; }
        public int? ServicePlanID { get; set; }
        public ServicePlan ServicePlan { get; set; }
        [ForeignKey("Operator")]
        public String OperatorID { get; set; }
        public Operator Operator { get; set; }
    }
}
