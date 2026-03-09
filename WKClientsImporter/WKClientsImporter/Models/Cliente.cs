using System;
using System.ComponentModel.DataAnnotations;
using WKClientsImporter.Models.Validators;

namespace WKClientsImporter.Models
{
    public class Cliente
    {
        [Required]
        [RegularExpression(@"^[0-9]{8}[A-Z]$", ErrorMessage = "DNI con formato incorrecto")]
        public string DNI { get; set; }
        
        [Required(ErrorMessage = "Nombre obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Apellidos obligatorios")]
        [StringLength(50)]
        public string Apellidos { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(ClienteValidator), "ValidateFechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email con formato incorrecto")]
        public string Email { get; set; }

        [RegularExpression(@"^[0-9]{9,15}$", ErrorMessage = "Telefono con formato incorrecto")]
        public string Telefono { get; set; }

    }
}
