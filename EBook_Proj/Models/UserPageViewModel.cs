namespace EBook_Proj.Models;

public class UserPageViewModel
{
    public OrderedBooksModel OrderedBooks { get; set; } = new();
    public SiteReviewModel SiteReview { get; set; } = new();
}