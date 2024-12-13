using System.ComponentModel.DataAnnotations;

public class UserModel
{
    [Key]
    public int CustomerID { get; set; }  // This is primary key
    [Required(ErrorMessage = "Please Enter First Name")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Please Enter Last Name")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Please Email Address")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Please Enter Password")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", 
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; set; }
}