using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBook_Proj.Models;
[PrimaryKey(nameof(BookRevID), nameof(UserID), nameof(BookID))]

public class BookReview
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookRevID { get; set; }
    
    public int UserID { get; set; }
    
    [Required]
    public int BookID { get; set; }
    
    [Required(ErrorMessage = "Please rate the book.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rate { get; set; }
    
    [Required(ErrorMessage = "Please add a description.")]
    [MinLength(10, ErrorMessage = "Review must be at least 10 characters long")]
    public string Description { get; set; }
    
    public virtual UserModel User { get; set; }
}