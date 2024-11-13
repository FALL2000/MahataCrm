using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahataCrm.Models
{
    [NotMapped]
    [Keyless]
    public class ZreportViewModel
    {
        public string ReportNumber { get; set; }
        public string ReportDate { get; set; }
        public string ReportTime { get; set; }
        public string SubTotal { get; set; }
        public string Discount { get; set; }
        public string Total { get; set; }
        public string Vat {  get; set; }
        public string TotalGross { get; set; }
    }
}
