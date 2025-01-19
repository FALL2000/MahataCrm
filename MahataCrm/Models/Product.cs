using CrmMahata.Models;
using System.ComponentModel.DataAnnotations;

namespace MahataCrm.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [Range(1, double.MaxValue)]
        public double Price { get; set; }
        public int AccountID { get; set; }
        public string Category { get; set; }
        public Account? Account { get; set; }

    }
}
