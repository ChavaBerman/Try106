﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BOL.Validations
{
    class ValidDateTimeEndAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
           
                //Take userId and email of the user parameter
                DateTime dateBegin = (validationContext.ObjectInstance as Project).DateBegin;
          
                if (dateBegin>((DateTime)value))
                {
                    ErrorMessage = "the end date of project is grater than begin date.";
                    validationResult = new ValidationResult(ErrorMessageString);
                }
           
            return validationResult;
        }

    }
}
