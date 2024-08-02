using System.ComponentModel.DataAnnotations;

namespace MahataCrm.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Action { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAction { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime TimeAction { get; set; }
        public string OperatorID { get; set; }
        public Operator Operator { get; set; }
        public string ActionOn { get; set; }
    }
}
