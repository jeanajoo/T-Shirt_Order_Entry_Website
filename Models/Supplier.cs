using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Joo_Jeana_HW5.Models
{
    public class Supplier
    {
        // Scalar Properties
        [Key]
        public Int32 SupplierID { get; set; } 

        [Required(ErrorMessage = "Supplier name is required!")]
        [Display(Name = "Supplier Name")]
        public String SupplierName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Phone number is required!")]
        [Display(Name = "Phone Number")]
        public String PhoneNumber { get; set; }

        // Navigational Properties
        public List<Product>? Products { get; set; }
    }
}