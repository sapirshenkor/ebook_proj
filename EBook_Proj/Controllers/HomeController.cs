using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EBook_Proj.Models;
using EBook_Proj.DATA;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.Models;
using System.Linq;
using EBook_Proj.Services;


namespace EBook_Proj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailService emailService)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        await CheckBooksToReturn();
        // Get all books ordered by newest first, take 8 for the home page
        var viewModel = new HomePageBooksViewModel
        {
            TotalBooks = await _context.Books.CountAsync(), 
            FeaturedBooks = await _context.Books
                .OrderByDescending(b => b.PublicationDate)
                .Take(8)
                .ToListAsync(),

            PopularBooks = await _context.Books
                .OrderByDescending(b => b.BorrowCount)
                .Take(4)
                .ToListAsync(),
            
            SiteReviews = await _context.SiteReview
            .Include(r => r.User)
            .ToListAsync()
        };

        
        return View(viewModel);
    }
    

    private async Task CheckBooksToReturn()
    {
        try
        {
            var booksToReturn = await _context.BooksUser
                .Where(bu => bu.Type == "borrow" && bu.Date.AddDays(25).Date <= DateTime.Now.Date).ToListAsync();
            foreach (var bookUser in booksToReturn)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.CustomerID == bookUser.UserID);
                if (user == null) continue;
                var book = await _context.Books.FirstOrDefaultAsync(b => b.BookID == bookUser.BookID);
                if (book == null) continue;
                
                
                string subject = "Return Reminder: Your Book is Due Today!";
                string body = $@"
                    <h2>Hello {user.FirstName} {user.LastName}!</h2>
                    <p>This is a  reminder that the following book is due to be returned today:</p>
                    <div style='margin: 20px; padding: 15px; background-color: #f5f5f5; border-left: 4px solid #007bff;'>
                        <h3>{book.Title}</h3>
                    </div>
                    <p>Please return it to the library as soon as possible to avoid any late fees.</p>
                    <p>With 5 days the book will be returned automatically!</p>
                    <p>Thank you for using our service!</p>
                    <br>
                    <p>Best regards,</p>
                    <p>The EBook Store Team</p>";

                    await _emailService.SendEmailAsync(user.Email, subject, body, isHtml: true);
            }
            var booksReturn = await _context.BooksUser
                .Where(bu => bu.Type == "borrow" && bu.Date.AddDays(30).Date == DateTime.Now.Date).ToListAsync();
            foreach (var bookUser in booksReturn)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.CustomerID == bookUser.UserID);
                if (user == null) continue;
                var book = await _context.Books.FirstOrDefaultAsync(b => b.BookID == bookUser.BookID);
                if (book == null) continue;
                
                
                string subject = "Return Reminder: Your Book is Due Today!";
                string body = $@"
                    <h2>Hello {user.FirstName} {user.LastName}!</h2>
                    <p>We want to inform you that the following book has been automatically returned:</p>
                    <div style='margin: 20px; padding: 15px; background-color: #f5f5f5; border-left: 4px solid #dc3545;'>
                        <h3>{book.Title}</h3>
                    </div>
                    <p>The book was returned automatically as the period has expired.</p>
                    <p>If you'd like to borrow this book again, please visit our library and make a new request.</p>
                    <p>Thank you for using our service!</p>
                    <br>
                    <p>Best regards,</p>
                    <p>The EBook Store Team</p>";

                await _emailService.SendEmailAsync(user.Email, subject, body, isHtml: true);
                _context.BooksUser.Remove(bookUser);
                await _context.SaveChangesAsync();
                book.BorrowCount = book.BorrowCount + 1;
                if (book.BorrowCount - 1 == 0)
                {
                    var allUserToNote = await _context.WaitingList.Where(w => w.BookID == book.BookID)
                        .OrderBy(w=>w.Date).ToListAsync();
                    for (int i = 0; i < allUserToNote.Count; i++)
                    {
                        var note = allUserToNote[i];
                        var usertonote = await _context.Users.FirstOrDefaultAsync(u => u.CustomerID == note.UserID);
                        try
                        {
                            await _emailService.NotifyWaitingListUsersAsync(book, usertonote);
                            _context.WaitingList.Remove(note);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to send email to user {note.Email}: {ex.Message}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking books to return");
        }
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