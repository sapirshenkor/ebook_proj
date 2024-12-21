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
        // Get all books ordered by newest first, take 8 for the home page
        var viewModel = new HomePageBooksViewModel
        {
            FeaturedBooks = await _context.Books
                .OrderByDescending(b => b.PublicationDate)
                .Take(8)
                .ToListAsync(),

            PopularBooks = await _context.Books
                .OrderByDescending(b => b.BorrowCount)
                .Take(4)
                .ToListAsync()
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