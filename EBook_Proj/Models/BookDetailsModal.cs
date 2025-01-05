namespace EBook_Proj.Models;

public class BookDetailsModal
{
    public Books Book { get; set; }
    public IEnumerable<BookReview> Reviews { get; set; }
    public IEnumerable<BooksUserModel> BooksUser { get; set; }
}