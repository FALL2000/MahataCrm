
using MahataCrm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace CrmMahata.Models
{

    public enum StatusType
    {
        Active,
        Inactive
    }

    
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Tin), IsUnique = true)]
    [Index(nameof(Certkey), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public class Account
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public StatusType Status { get; set; }
        public int Tin { get; set; }
        public string Certkey { get; set; }
        public string RegId { get; set; }
        public string Vrn { get; set; }
        public string Serial { get; set; }
        public string Uin { get; set; }
        public string TaxOffice {  get; set; }
        [DataType(DataType.Password)]
        [StringLength(12, ErrorMessage = "The password must be at least 8 and at max 12 characters long.", MinimumLength = 8)]
        [NotMapped]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedAt { get; set; }
        public int? ServicePlanID { get; set; }
        public ServicePlan ServicePlan { get; set; }
        [ForeignKey("Operator")]
        public string OperatorID { get; set; }
        public Operator Operator { get; set; }
        public ICollection<Receipt> Receipts { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
