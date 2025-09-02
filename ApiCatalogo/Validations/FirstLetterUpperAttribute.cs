using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.Validations
{
    public class FirstLetterUpperAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var primeiraLetra = value.ToString()[0].ToString();
            if (primeiraLetra != primeiraLetra.ToUpper())
                return new ValidationResult("A primeira letra do produto precisa ser maiuscula");
            return ValidationResult.Success;
        }
    }
}
