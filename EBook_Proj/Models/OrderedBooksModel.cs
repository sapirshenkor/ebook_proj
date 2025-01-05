namespace EBook_Proj.Models;

public class OrderedBooksModel
{
    public List<Books> OwnedBooks { get; set; } = [];
    public List<BookWithDateModel> BorrowedBooks { get; set; } = [];
}