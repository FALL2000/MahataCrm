using CrmMahata.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahataCrm.Models
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public EnumDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    public enum PaymentType
    {
        [EnumDisplayName("CASH")]
        CASH,
        [EnumDisplayName("INVOICE")]
        INVOICE,
        [EnumDisplayName("CHEQUE")]
        CHEQUE,
        [EnumDisplayName("CCARD")]
        CCARD,
        [EnumDisplayName("EMONEY")]
        EMONEY
    }

    public class Receipt
    {
        public int Id { get; set; }
        public string RctNum { get; set; }
        public string? Znum { get; set;}
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RctDate { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan RctTime { get; set; }
        public double? TotalTaxExcl { get; set; }
        public double? TotalTaxIncl { get; set; }
        public double? TotalTax { get; set; }
        public string? LinkVerification { get; set; }
        public string CustName {  get; set; }
        public string CustIdType { get; set; }
        public string CustId { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool isPost { get; set; }
        public int? CustNum { get; set; }
        public int AccountID { get; set; }
        public Account Account { get; set; }
        public ICollection<ReceiptItem> ReceiptItems { get; set; }

    }
}
