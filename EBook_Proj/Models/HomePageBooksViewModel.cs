﻿namespace EBook_Proj.Models;
using  EBook_Proj.Models;  


public class HomePageBooksViewModel
{
    public HomePageBooksViewModel()
    {
        FeaturedBooks = new List<Books>();
        PopularBooks = new List<Books>();
        Genres = new List<string>();
        Authors = new List<string>();
        SiteReviews = new List<SiteReviewModel>();
        
    }
    public IEnumerable<Books> FeaturedBooks { get; set; }
    public IEnumerable<Books> PopularBooks { get; set; }
    public IEnumerable<string> Genres { get; set; }
    public IEnumerable<string> Authors { get; set; }
    public String SelectedAuthor { get; set; }
    public string SearchString { get; set; }
    public string SelectedGenre { get; set; }
    public IEnumerable<SiteReviewModel> SiteReviews { get; set; }
    public int TotalBooks { get; set; }
    public string SortOption { get; set; }
    public List<int> PopularBookIds { get; set; }

}