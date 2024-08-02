using CrmMahata.Models;
using Microsoft.EntityFrameworkCore;

namespace MahataCrm.Models
{
    [Index(nameof(BussinessId), IsUnique = true)]
    public class OperatorMatchAccount
    {
        public int Id { get; set; }
        public String OperatorID { get; set; }
        public Operator Operator { get; set; }
        public int BussinessId { get; set; }
    }
}
