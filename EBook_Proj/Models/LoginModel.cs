using System.ComponentModel.DataAnnotations;

namespace EBook_Proj.Models;

public class LoginModel
{
    // [Required(ErrorMessage = "Email is required")]
    // [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }

    
    public string? Password { get; set; }
}