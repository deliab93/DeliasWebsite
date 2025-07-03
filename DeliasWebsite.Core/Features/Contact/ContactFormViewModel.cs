using System.ComponentModel.DataAnnotations;

namespace DeliasWebsite.Core.Features.Contact
{

    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Your name is required")]
        public string Name { get; set; }

        [Required, EmailAddress(ErrorMessage = "A valid email is required")]
        public string Email { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter a message")]
        public string Message { get; set; }
    }
    
}
