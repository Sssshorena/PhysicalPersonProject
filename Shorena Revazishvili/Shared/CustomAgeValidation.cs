using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace Shared
{
    public class CustomAgeValidation : ValidationAttribute, IClientModelValidator
    {
        public int MinimumAge { get; }

        public CustomAgeValidation(int minimumAge = 18)
        {
            MinimumAge = minimumAge;
            ErrorMessage = $"You must be at least {MinimumAge} years old.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null) return false;

            if (value is DateTime birthDate)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age))
                { 
                    age--; 
                }

                return age >= MinimumAge;
            }
            return false;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-customAge", ErrorMessage);
            context.Attributes.Add("data-val-customAge-minDate", DateTime.Today.AddYears(-MinimumAge).ToString("yyyy-MM-dd"));
        }
    }
}
