namespace EBook_Proj.Models;

public class HomePageBooksViewModel
{
    public HomePageBooksViewModel()
    {
        FeaturedBooks = new List<Books>();
        PopularBooks = new List<Books>();
        Genres = new List<string>();
        
    }
    public IEnumerable<Books> FeaturedBooks { get; set; }
    public IEnumerable<Books> PopularBooks { get; set; }
    public IEnumerable<string> Genres { get; set; }
    public string SearchString { get; set; }
    public string SelectedGenre { get; set; }
}