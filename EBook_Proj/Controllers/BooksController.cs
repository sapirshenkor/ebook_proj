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
    public async Task<IActionResult> BooksCatalog(string searchString, string genre, string priceSort, string yearSort,string author, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Books.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(b => 
                b.Title.Contains(searchString) || 
                b.Author.Contains(searchString) || 
                b.Description.Contains(searchString));
        }

        // Apply genre filter
        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(b => b.Genre == genre);
        }
        // Apply author filter
        if (!string.IsNullOrEmpty(author))
        {
            query=query.Where(b => b.Author == author);
        }
        // Price range filter
        if (minPrice.HasValue)
        {
            query = query.Where(b => b.BuyingPrice >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(b => b.BuyingPrice <= maxPrice.Value);
        }

        // Apply price sorting
        switch (priceSort?.ToLower())
        {
            case "asc":
                query = query.OrderBy(b => b.BuyingPrice);
                break;
            case "desc":
                query = query.OrderByDescending(b => b.BuyingPrice);
                break;
        }

        // Apply year sorting
        switch (yearSort?.ToLower())
        {
            case "asc":
                query = query.OrderBy(b => b.PublicationDate);
                break;
            case "desc":
                query = query.OrderByDescending(b => b.PublicationDate);
                break;
        }

        var viewModel = new HomePageBooksViewModel
        {
            FeaturedBooks = await query.ToListAsync(),
            Genres = await _context.Books.Select(b => b.Genre).Distinct().ToListAsync(),
            Authors = await _context.Books.Select(b => b.Author).Distinct().ToListAsync(),
            SelectedAuthor = author,
            SearchString = searchString,
            SelectedGenre = genre
        };

        return View(viewModel);
    }

    public async Task<IActionResult> BookDetails(int id)
    {
        
        
        var reviews = await _context.BookReview
            .Include(r => r.User)  // Include the User navigation property
            .Where(r => r.BookID == id)
            .ToListAsync();
        var book= await _context.Books.FirstOrDefaultAsync(b=>b.BookID == id);
        List<BooksUserModel> booksUserModels = new List<BooksUserModel>();
        if (book == null)
        {
            return NotFound();
        }
        List<BooksUserModel> booksusers = new List<BooksUserModel>();

        if (HttpContext.Session.GetString("CustomerID") != null)
        {
            var userIdString = HttpContext.Session.GetString("CustomerID");
            if (!string.IsNullOrEmpty(userIdString))
            {
                var customerID = int.Parse(userIdString);
                booksusers = await _context.BooksUser
                    .Where(b => b.UserId == customerID)
                    .ToListAsync();
            }
        }

        var bookdetails = new BookDetailsModal
        {
            Reviews = reviews,
            Book = book,
            BooksUser = booksusers
        };
        return View(bookdetails);

        
    }
    [HttpGet]
    public IActionResult Download(string format)
    {
        byte[] fileBytes;
        string contentType;
        string fileName;

        // Create empty file bytes - just a small placeholder
        fileBytes = new byte[] { 0x0 };

        switch (format.ToLower())
        {
            case "epub":
                contentType = "application/epub+zip";
                fileName = $"book.epub";
                break;
            case "fb2":
                contentType = "application/xml";
                fileName = $"book.fb2";
                break;
            case "mobi":
                contentType = "application/x-mobipocket-ebook";
                fileName = $"book.mobi";
                break;
            case "pdf":
                contentType = "application/pdf";
                fileName = $"book.pdf";
                break;
            default:
                return BadRequest("Unsupported format");
        }

        return File(fileBytes, contentType, fileName);
    
    }
    [HttpPost]
    public async Task<IActionResult> AddReview(BookReview review)
    {
        ModelState.Remove("User");

        if (ModelState.IsValid)
        {
            review.UserID = int.Parse(HttpContext.Session.GetString("CustomerID"));
            _context.BookReview.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BookDetails), new { id = review.BookID });
        }
        return RedirectToAction(nameof(BookDetails), new { id = review.BookID });
    }
    
    
}
