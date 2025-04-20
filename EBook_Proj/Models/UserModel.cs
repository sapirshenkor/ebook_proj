using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
public class UserModel
{
    [Key]
    public int CustomerID { get; set; }  // This is primary key
    [Required(ErrorMessage = "Please Enter First Name")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Please Enter Last Name")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Please Email Address")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [Display(Name = "Email")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Please Enter Password")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", 
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    [StringLength(100)]
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
    public string? TemporaryPassword { get; set; }

    public DateTime? TemporaryPasswordExpiration { get; set; }
}