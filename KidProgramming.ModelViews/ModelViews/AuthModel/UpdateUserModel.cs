using KidPrograming.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace KidProgramming.ModelViews.ModelViews.AuthModel
{
    public class UpdateUserModel
    {
        public string? FullName { get; set; }

        [RegularExpression(@"^(0|\+84)[3-9][0-9]{8}$", ErrorMessage = "Phone number is not validation")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(UpdateUserModel), nameof(ValidateDateOfBirth))]
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }

        [StringLength(36, ErrorMessage = "Parent ID must be a valid GUID.")]
        public string? ParentId { get; set; }

        public static ValidationResult? ValidateDateOfBirth(DateTimeOffset? dateOfBirth, ValidationContext context)
        {
            if (dateOfBirth.HasValue)
            {
                if (dateOfBirth > CoreHelper.SystemTimeNow)
                {
                    return new ValidationResult("Date of birth cannot be in the future.");
                }

                if (dateOfBirth < DateTimeOffset.UtcNow.AddYears(-100)) 
                {
                    return new ValidationResult("Date of birth is too old.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
