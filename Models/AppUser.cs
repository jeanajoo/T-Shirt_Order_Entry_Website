using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Joo_Jeana_HW5.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }
    }
}