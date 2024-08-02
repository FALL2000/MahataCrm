using CrmMahata.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahataCrm.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        public string RctNum { get; set; }
        public string Znum { get; set;}
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RctDate { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan RctTime { get; set; }
        public double TotalTaxExcl { get; set; }
        public double TotalTaxIncl { get; set; }
        public string CustName {  get; set; }
        public string CustIdType { get; set; }
        public string CustId { get; set; }
        public int? CustNum { get; set; }
        public int AccountID { get; set; }
        public Account Account { get; set; }
        public ICollection<ReceiptItem> ReceiptItems { get; set; }

    }
}
