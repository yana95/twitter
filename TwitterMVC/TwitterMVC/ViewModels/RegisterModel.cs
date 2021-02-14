using System.ComponentModel.DataAnnotations;

namespace TwitterMVC.ViewModels
{
    public class RegisterModel
    {
        [EmailAddress(ErrorMessage = "Invalid email")]
        [Required(ErrorMessage = "Field is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [MaxLength(20, ErrorMessage = "Lenght should be less than 20 symbols")]
        [MinLength(6, ErrorMessage = "Lenght should be more than 20 symbols")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Field is required")]
        [Compare("Password", ErrorMessage = "Passwords are different")]
        public string ConfirmPassword { get; set; }
    }
}
