using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MahataCrm.Models
{
    [NotMapped]
    [Keyless]
    public class SignatureModel
    {
        public string DistinguishedName { get; set; }

        public string SerialNumber { get; set; }

        public string Signature { get; set; }
    }
}
