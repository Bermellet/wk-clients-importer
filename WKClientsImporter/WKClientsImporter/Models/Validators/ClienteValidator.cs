using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WKClientsImporter.Models.Validators
{
    public static class ClienteValidator
    {
        public static ValidationResult ValidateFechaNacimiento(DateTime? date, ValidationContext context)
        {
            if (date > DateTime.Now)
            {
                return new ValidationResult("Fecha de Nacimiento no puede ser futura");
            }
            return ValidationResult.Success;
        }

        public static bool TryValidate(Cliente cliente, out List<string> errors)
        {
            var context = new ValidationContext(cliente);
            var results = new List<ValidationResult>();
            errors = new List<string>();

            bool isValid = Validator.TryValidateObject(cliente, context, results, true);

            if (!isValid)
            {
                foreach (var error in results) errors.Add(error.ErrorMessage);
            }

            return isValid;
        }

        public static List<ValidationResult> GetValidationResults(Cliente cliente)
        {
            if (cliente == null) return new List<ValidationResult>();

            var context = new ValidationContext(cliente);
            var results = new List<ValidationResult>();

            // Importante validateAllProperties = true para recoger todas las anotaciones
            Validator.TryValidateObject(cliente, context, results, true);

            return results;
        }
    }
}
