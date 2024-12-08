﻿using System.ComponentModel.DataAnnotations;

namespace PROJECT_SEM3.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "the {0} must be at {2} and at max{1} character long.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match.")]

        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required.")]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm New Password")]

        public string ConfirmNewPassword { get; set; }
    }
}
