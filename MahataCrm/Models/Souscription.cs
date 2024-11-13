using CrmMahata.Models;
using System.ComponentModel.DataAnnotations;

namespace MahataCrm.Models
{
    public class Souscription
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSouscription { get; set; }

        public int ServicePlanID { get; set; }
        public ServicePlan ServicePlan { get; set; }

        public int AccountID { get; set; }
        public Account Account { get; set; }
    }
}
