using EBook_Proj.DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.Models;

namespace EBook_Proj.Controllers;

public class BooksController: Controller
{
    //this field holds the dataBase connection
    private readonly ApplicationDbContext _context;
    
    //constructor = gets database context through dependency injection
    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }
    // Get all books and resent them in the books catalog
    public async Task<IActionResult> BooksCatalog(string searchString, string genre)
    {
        // Get unique genres from the database
        var genres = await _context.Books
            .Select(b => b.Genre)
            .Distinct()
            .Where(g => !string.IsNullOrEmpty(g))
            .ToListAsync();

        // Start with all books
        var booksQuery = _context.Books.AsQueryable();

        // Apply search filters if provided
        if (!string.IsNullOrEmpty(searchString))
        {
            booksQuery = booksQuery.Where(b => 
                b.Title.Contains(searchString) || 
                b.Author.Contains(searchString) || 
                b.Description.Contains(searchString));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            booksQuery = booksQuery.Where(b => b.Genre == genre);
        }

        var viewModel = new HomePageBooksViewModel
        {
            FeaturedBooks = await booksQuery.ToListAsync(),
            Genres = genres,
            SearchString = searchString,
            SelectedGenre = genre
        };

        return View(viewModel);
    }
    
    
}
