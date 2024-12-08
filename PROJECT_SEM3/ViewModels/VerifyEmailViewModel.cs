using System.ComponentModel.DataAnnotations;

namespace PROJECT_SEM3.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]

        public string Email { get; set; }
    }
}
