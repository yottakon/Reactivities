using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        //Required puts validation to make sure new users fll these fields as a certain type
        [Required]
        public string DisplayName { get; set; }
        //Email checks if it really is an email
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //Regular expression means that it need a number, lower and uppercase letter, and be 4-8 characters long 
        [Required]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; }
        [Required]
        public string Username { get; set; }
    }
}