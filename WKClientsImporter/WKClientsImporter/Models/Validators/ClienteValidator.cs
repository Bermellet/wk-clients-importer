using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WKClientsImporter.Models.Validators
{
    public static class ClienteValidator
    {
        public static ValidationResult ValidateFechaNacimiento(DateTime date, ValidationContext context)
        {
            if (date > DateTime.Now)
            {
                return new ValidationResult("Fecha de Nacimiento no puede ser futura");
            }
            return ValidationResult.Success;
        }

        // Método genérico para validar cualquier objeto Cliente
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
    }
}
