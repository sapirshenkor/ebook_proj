namespace EBook_Proj.Models;

public class OrderedBooksModel
{
    public List<Books> OwnedBooks { get; set; } = [];
    public List<Books> BorrowedBooks { get; set; } = [];
}