using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliasWebsite.Core.Features.Contact
{
    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Subject { get; set; } = "";

        [Required]
        public string Message { get; set; } = "";
    }
}
