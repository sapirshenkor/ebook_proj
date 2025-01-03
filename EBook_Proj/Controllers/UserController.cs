using EBook_Proj.DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.Models;


namespace EBook_Proj.Controllers;

public class UserController : Controller
{
    //this field holds the dataBase connection
    private readonly ApplicationDbContext _context;
    
    //constructor = gets database context through dependency injection
    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create (UserModel user)
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
            // Get the orders and log the count
            var orders = await _context.Orders
                .Where(o => o.CustomerID == int.Parse(customerID))
                .Select(o => o.OrderID)
                .ToListAsync();
        
            Console.WriteLine($"Found {orders.Count} orders"); // Debug line

            // Get order details and log the count
            var books = await _context.OrderDetails
                .Where(od => orders.Contains(od.OrderID))
                .ToListAsync();
            
            Console.WriteLine($"Found {books.Count} order details"); // Debug line

            var bookIDBuy = books
                .Where(od => od.BookType == "buy")
                .Select(od => od.BookID)
                .ToList();
            
            Console.WriteLine($"Found {bookIDBuy.Count} bought books"); // Debug line

            var bookIDBorrow = books
                .Where(od => od.BookType == "borrow")
                .Select(od => od.BookID)
                .ToList();
            
            Console.WriteLine($"Found {bookIDBorrow.Count} borrowed books"); // Debug line

            viewModel.OrderedBooks.OwnedBooks = await _context.Books
                .Where(b => bookIDBuy.Contains(b.BookID))
                .ToListAsync();

            viewModel.OrderedBooks.BorrowedBooks = await _context.Books
                .Where(b => bookIDBorrow.Contains(b.BookID))
                .ToListAsync();
        
            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}"); // Debug line
            return View(viewModel);
        }
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
}