using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace EBook_Proj.Models;

[PrimaryKey(nameof(BookID), nameof(UserID))]
public class BooksUserModel
{
    [Required]
    public int BookID { get; set; }
    [Required]
    public int UserID { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public  DateTime Date { get; set; }
    
    
}