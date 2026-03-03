using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WKClientsImporter.Models.Validators
{
    public static class CustomerValidator
    {
        public static ValidationResult ValidateBirthdate(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Now)
            {
                return new ValidationResult("Birthdate cannot be future");
            }
            return ValidationResult.Success;
        }

        // Método genérico para validar cualquier objeto Customer
        public static bool TryValidate(Customer customer, out List<string> errors)
        {
            var context = new ValidationContext(customer);
            var results = new List<ValidationResult>();
            errors = new List<string>();

            bool isValid = Validator.TryValidateObject(customer, context, results, true);

            if (!isValid)
            {
                foreach (var error in results) errors.Add(error.ErrorMessage);
            }

            return isValid;
        }
    }
}
