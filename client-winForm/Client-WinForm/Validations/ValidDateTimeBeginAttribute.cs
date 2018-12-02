using System;
using System.ComponentModel.DataAnnotations;

namespace Client_WinForm.Validations
{
    public class ValidDateTimeBeginAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            if (DateTime.Parse(value.ToString()) >= DateTime.Now)
                return null;
           return new ValidationResult("date begin project less than today");
        }

    }
}
