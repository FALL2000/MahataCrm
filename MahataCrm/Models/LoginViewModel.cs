using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MahataCrm.Models
{
    [Keyless]
    public class LoginViewModel
    {

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }
}
