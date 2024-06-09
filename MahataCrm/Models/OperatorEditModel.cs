using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MahataCrm.Models
{
    [Keyless]
    public class OperatorEditModel
    {
        public string? IdOp { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
