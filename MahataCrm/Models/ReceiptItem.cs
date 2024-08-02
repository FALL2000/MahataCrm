using System.ComponentModel.DataAnnotations;

namespace MahataCrm.Models
{
    public class ReceiptItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [Range(1, double.MaxValue)]
        public double Quantity { get; set; }
        [Range(1, double.MaxValue)]
        public double Price { get; set; }
        public bool IsNew { get; set; }
        public int? ReceiptID { get; set; }
        public Receipt Receipt { get; set; }
    }
}
