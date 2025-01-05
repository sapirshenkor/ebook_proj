using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EBook_Proj.Models;
[PrimaryKey(nameof(BookID), nameof(UserID))]
public class WaitingListModel
{
    [Required]
    public int BookID { get; set; }
    [Required]
    public int UserID { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public  DateTime Date { get; set; }
}