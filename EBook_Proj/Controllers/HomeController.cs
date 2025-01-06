using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EBook_Proj.Models;
using EBook_Proj.DATA;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.Models;
using System.Linq;


namespace EBook_Proj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    //this field holds the dataBase connection
    private readonly ApplicationDbContext _context;
    
    //constructor = gets database context through dependency injection
    

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // First get total books count
        var totalBooks = await _context.Books.CountAsync();

        // Get popular books from orders
        var popularBooks = await _context.OrderDetails
            .GroupBy(od => od.BookID)
            .Select(group => new
            {
                BookID = group.Key,
                OrderCount = group.Count()
            })
            .OrderByDescending(x => x.OrderCount)
            .Join(
                _context.Books,
                ordered => ordered.BookID,
                book => book.BookID,
                (ordered, book) => book
            )
            .Take(4)
            .ToListAsync();

        // If we have fewer than 4 popular books, supplement with recent books
        if (popularBooks.Count < 4)
        {
            var additionalBooks = await _context.Books
                .Where(b => !popularBooks.Select(ob => ob.BookID).Contains(b.BookID))
                .OrderByDescending(b => b.PublicationDate)
                .Take(4 - popularBooks.Count)
                .ToListAsync();
    
            popularBooks.AddRange(additionalBooks);
        }

        // Get featured books, excluding popular ones
        var featuredBooks = await _context.Books
            .Where(b => !popularBooks.Select(pb => pb.BookID).Contains(b.BookID))
            .OrderByDescending(b => b.PublicationDate)
            .Take(8)
            .ToListAsync();

        // Get site reviews
        var siteReviews = await _context.SiteReview
            .Include(r => r.User)
            .ToListAsync();

        // Create the view model
        var viewModel = new HomePageBooksViewModel
        {
            TotalBooks = totalBooks,
            FeaturedBooks = featuredBooks,
            PopularBooks = popularBooks,
            SiteReviews = siteReviews
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
}