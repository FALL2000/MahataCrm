using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace applicationcrm.Models
{
    public enum StatusType
    {
        Active,
        Inactive
    }

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
        public string? Country { get; set; }
        public string? City { get; set; }
        public int? Gc { get; set; }
        public string TaxOffice { get; set; }

     
    }
}