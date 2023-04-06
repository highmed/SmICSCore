using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public class WardParameter : Case
    {
        [Required]
        public string Ward { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        [EndDateValidator]
        public DateTime End { get; set; }
        public List<string> PathogenCode { get; set; }
        public string DepartementID { get; set; }
    }

    public class EndDateValidator : ValidationAttribute
    {
        protected override ValidationResult
                IsValid(object value, ValidationContext validationContext)
        {
            var model = (WardParameter)validationContext.ObjectInstance;
            DateTime StartDate = Convert.ToDateTime(model.Start);
            DateTime EndDate = Convert.ToDateTime(value);
            if (EndDate < StartDate)
            {
                return new ValidationResult
                    ("Das Enddatum nach dem Startdatum liegen!");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}