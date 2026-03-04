using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WKClientsImporter.Models.Validators;

namespace WKClientsImporter.Models
{
    public class Customer
    {
        [Required]
        [RegularExpression(@"^[0-9]{8}[A-Z]$", ErrorMessage = "DNI format incorrect")]
        public string DNI { get; set; }
        
        [Required(ErrorMessage = "Name is mandatory")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Lastname is mandatory")]
        [StringLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(CustomerValidator), "ValidateBirthdate")]
        public DateTime Birthdate { get; set; }

        [RegularExpression(@"^[0-9]{9,15}$", ErrorMessage = "Phone format incorrect")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email format invalid")]
        public string Email { get; set; }


    }
}
