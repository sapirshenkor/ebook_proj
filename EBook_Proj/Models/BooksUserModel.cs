using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace EBook_Proj.Models;

[PrimaryKey(nameof(BookId), nameof(UserId))]
public class BooksUserModel
{
    [Required]
    public int BookId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public  DateTime Date { get; set; }
    
    
}