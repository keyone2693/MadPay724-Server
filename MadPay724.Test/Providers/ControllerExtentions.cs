using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace MadPay724.Test.Providers
{
    public static class ControllerExtentions
    {
        public static void ValidateModelState<TViewModel, TController>(this TController controller, TViewModel viewModelToValidate)
            where TController : Controller
        {
            var validationContext = new ValidationContext(viewModelToValidate, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(viewModelToValidate, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.FirstOrDefault() ?? string.Empty, validationResult.ErrorMessage);
            }
        }
    }
}
