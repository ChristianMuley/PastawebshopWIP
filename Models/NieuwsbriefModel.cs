using System;
using System.ComponentModel.DataAnnotations;

namespace Pastashop.Models
{
    public class NieuwsbriefModel
    {
        [Required(ErrorMessage = "Voornaam is verplicht.")]
        public string Voornaam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Naam { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mailadres is verplicht.")]
        [EmailAddress(ErrorMessage = "Geef een geldig e-mailadres in.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefoonnummer is verplicht.")]
        [Phone(ErrorMessage = "Geef een geldig telefoonnummer in.")]
        public string Telefoonnummer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Geboortedatum is verplicht.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(NieuwsbriefModel), nameof(ValidatePastDate))]
        public DateTime? Geboortedatum { get; set; }

        // Validatie om te checken als de datum in het verleden is
        public static ValidationResult? ValidatePastDate(DateTime? geboortedatum, ValidationContext context)
        {
            if (!geboortedatum.HasValue)
                return ValidationResult.Success;

            return geboortedatum.Value < DateTime.Today
                ? ValidationResult.Success
                : new ValidationResult("Geboortedatum moet in het verleden liggen.");
        }
    }
}