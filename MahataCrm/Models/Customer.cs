using CrmMahata.Models;
using System.ComponentModel.DataAnnotations;

namespace MahataCrm.Models
{

    public enum CustIdType
    {
        [EnumDisplayName("TIN")]
        TIN,
        [EnumDisplayName("Driving License")]
        DrivingLicense,
        [EnumDisplayName("Voters Number")]
        VotersNumber,
        [EnumDisplayName("Passport")]
        PassPort,
        [EnumDisplayName("NIDA")]
        NIDA,
        [EnumDisplayName("Telephone")]
        Telephone
    }

    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public CustIdType CustIdType { get; set; }
        [Required]
        public string CustId { get; set; }
        public int? CustNum { get; set; }
        public int AccountID { get; set; }
        public Account? Account { get; set; }
    }
}
