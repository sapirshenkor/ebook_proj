using System.ComponentModel.DataAnnotations;

namespace EBook_Proj.Models;

public class SiteReviewModel
{
    [Key]
    public int RevID { get; set; }
    public int UserID { get; set; }
    [Required(ErrorMessage = "Please rate the site.")]
    public int Rate { get; set; }
    [Required(ErrorMessage = "Please add description.")]
    public string Description { get; set; }
}