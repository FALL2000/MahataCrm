using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahataCrm.Models
{
    [NotMapped]
    [Keyless]
    public class OperatorCreateViewModel
    {

        public string? IdOp { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        [EmailAddress] 
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(12, ErrorMessage = "The password must be at least 8 and at max 12 characters long.", MinimumLength = 8)]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }

        [Required]
        public string Role {  get; set; }
    }
}
