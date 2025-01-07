using EBook_Proj.DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.Models;
using EBook_Proj.Services;


namespace EBook_Proj.Controllers;

public class UserController : Controller
{
    //this field holds the dataBase connection
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    
    //constructor = gets database context through dependency injection
    public UserController(ApplicationDbContext context, IEmailService emailService,ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = context;
        _emailService = emailService;
        
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserModel user)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already taken");
                return View(user);
            }
            
            _context.Add(user);
            await _context.SaveChangesAsync();

            // Send welcome email
            try
            {
                string subject = "Welcome to EBook Store!";
                string body = $@"
                    <h2>Welcome {user.FirstName} {user.LastName}!</h2>
                    <p>Thank you for creating an account with EBook Store.</p>
                    <p>You can now:</p>
                    <ul>
                        <li>Browse our extensive collection of books</li>
                        <li>Purchase or borrow books</li>
                        <li>Manage your digital library</li>
                    </ul>
                    <p>If you have any questions, feel free to contact our support team.</p>
                    <p>Happy reading!</p>
                    <br>
                    <p>Best regards,</p>
                    <p>The EBook Store Team</p>";

                await _emailService.SendEmailAsync(user.Email, subject, body, isHtml: true);
            }
            catch (Exception ex)
            {
                // Log the error but don't stop the registration process
                // You might want to add proper logging here
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
            }

            return RedirectToAction("Index", "Home");
        }
        return View(user);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel login)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email == login.Email && u.Password == login.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
            }
            else if (user.IsAdmin == true)
            {
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName); 
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
                return RedirectToAction("AdminPage", "Admin");
            }
            else
            {
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("CustomerID", user.CustomerID.ToString());
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                return RedirectToAction("UserPage", "User");
            }
            
        }
        return View();
    }
    [HttpPost]
    public IActionResult Logout()
    {
        // Clear all session data
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> SiteReview(UserPageViewModel model)
    {
        ModelState.Remove("SiteReview.User");
        if (ModelState.IsValid)
        {
            var customerIDString = HttpContext.Session.GetString("CustomerID");
            if (string.IsNullOrEmpty(customerIDString))
            {
                return RedirectToAction("Login", "User");
            }
        
            model.SiteReview.UserID = int.Parse(customerIDString);
            _context.Add(model.SiteReview);
            await _context.SaveChangesAsync();
            TempData["ReviewSuccess"] = true;
            return RedirectToAction("UserPage", "User");
        }
    
        return View("UserPage", model);
    }

    public IActionResult Profile()
    {
        return RedirectToAction("UserPage", "User");
    }

    [HttpGet]
    public async Task<IActionResult> UserPage()
    {
        var viewModel = new UserPageViewModel();
        var customerID = HttpContext.Session.GetString("CustomerID");
        if (customerID == null)
        { 
            return RedirectToAction("Login");
        }

        try
        {
            //get all books from BookUser for the userPage
            var allBooks = await _context.BooksUser
                .Where(b => b.UserID == int.Parse(customerID)).ToListAsync();
            
            var bookIDBuy = allBooks
                .Where(od => od.Type == "buy")
                .Select(od => od.BookID)
                .ToList();
            
            

            var bookIDBorrow = allBooks
                .Where(od => od.Type == "borrow")
                .Select(od => od.BookID)
                .ToList();
            
            
            HttpContext.Session.SetString("BorrowCount", bookIDBorrow.Count.ToString());
            viewModel.OrderedBooks.OwnedBooks = await _context.Books
                .Where(b => bookIDBuy.Contains(b.BookID))
                .ToListAsync();

            viewModel.OrderedBooks.BorrowedBooks = await _context.BooksUser.Where(bu => bu.UserID == int.Parse(customerID) && bu.Type == "borrow")
                .Join(_context.Books,
                    bu => bu.BookID,
                    b => b.BookID,
                    (bu, b) => new BookWithDateModel
                    {
                        // Copy all properties from Books with Date 
                        BookID = b.BookID,
                        Title = b.Title,
                        Author = b.Author,
                        Publisher = b.Publisher,
                        PublicationDate = b.PublicationDate,
                        Genre = b.Genre,
                        AgeLimit = b.AgeLimit,
                        Description = b.Description,
                        BuyingPrice = b.BuyingPrice,
                        BorrowPrice = b.BorrowPrice,
                        BorrowCount = b.BorrowCount,
                        PageCount = b.PageCount,
                        BookCover = b.BookCover,
                        Discount = b.Discount,
                        BorrowDate = bu.Date
                    })
                .ToListAsync();
        
            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}"); // Debug line
            return View(viewModel);
        }
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try 
        {
            var userId = HttpContext.Session.GetString("CustomerID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }
            var userID = int.Parse(userId);

            var book = await _context.BooksUser
                .FirstOrDefaultAsync(b => b.BookID == id && b.UserID == userID);
            if (book == null)
            {
                return NotFound();
            }
            if (book.Type == "borrow")
            {
                
                var BorrowedBooks =await _context.Books.FirstOrDefaultAsync(b => b.BookID == book.BookID);
                BorrowedBooks.BorrowCount += 1;
                if (BorrowedBooks.BorrowCount - 1 == 0)
                {
                    var allUserToNote = await _context.WaitingList.Where(w => w.BookID == book.BookID)
                        .OrderBy(w=>w.Date).ToListAsync();
                    for (int i = 0; i < allUserToNote.Count; i++)
                    {
                        var note = allUserToNote[i];
                        var usertonote = await _context.Users.FirstOrDefaultAsync(u => u.CustomerID == note.UserID);
                        try
                        {
                            await _emailService.NotifyWaitingListUsersAsync(BorrowedBooks, usertonote);
                            _context.WaitingList.Remove(note);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to send email to user {note.Email}: {ex.Message}");
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }

            _context.BooksUser.Remove(book);
            await _context.SaveChangesAsync();
    
            TempData["DeleteSuccess"] = "Book successfully deleted";
            return RedirectToAction("UserPage", "User");
        }
        catch (Exception ex)
        {
            // Log the error
            TempData["Error"] = "Failed to delete the book";
            return RedirectToAction("UserPage", "User");
        }
    }

    
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
}