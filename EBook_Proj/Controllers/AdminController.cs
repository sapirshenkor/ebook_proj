using EBook_Proj.DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.Models;

namespace EBook_Proj.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> AdminPage()
    { 
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }

        // Basic counts
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.TotalProducts = await _context.Books.CountAsync();
        ViewBag.TotalOrders = await _context.Orders.CountAsync();
        ViewBag.TotalReviews = await _context.SiteReview.CountAsync();

        ViewBag.BorrowRevenue = await _context.OrderDetails
            .Where(od => od.BookType == "Borrow")
            .SumAsync(od => od.BookPrice);

        ViewBag.BuyRevenue = await _context.OrderDetails
            .Where(od => od.BookType == "Buy")
            .SumAsync(od => od.BookPrice);


        ViewBag.BorrowRevenue ??= 0;
        ViewBag.BuyRevenue ??= 0;


        ViewBag.RecentOrders = await _context.Orders
            .OrderByDescending(o => o.PurchaseDate)
            .Take(5)
            .ToListAsync();

        ViewBag.ActiveDiscounts = await _context.Books
            .Where(b => b.Discount > 0 && b.DiscountDate > DateTime.Now)
            .CountAsync();

        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> ShowUsers()
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }
        var AllUsers =await _context.Users.ToListAsync();
        return View(AllUsers);
    }

    [HttpGet]
    public async Task<IActionResult> ShowBooks()
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }
        var AllBooks =await _context.Books.ToListAsync();
        return View(AllBooks);
    }

    [HttpGet]
    public async Task<IActionResult> ShowReviews()
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }
        var allRev = await _context.SiteReview.ToListAsync();
        return View(allRev);
    }

    [HttpGet]
    public async Task<IActionResult> ShowOrders()
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ToListAsync();
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromForm] Books book)
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }

        
        if (book.Discount > 0)
        {
            book.DiscountDate = DateTime.Now.AddDays(7);
        }
        else
        {
            book.DiscountDate = DateTime.MinValue;
        }

        if (ModelState.IsValid)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBook(int BookID)
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }
        var book = await _context.Books.FindAsync(BookID);
        if (book == null)
        {
            return Json(new { success = false, message = "Book not found" });
        }
        
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetBookDetails(int id)
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return Json(book);
    }

    [HttpPost]
    public async Task<IActionResult> EditBook(int id, [FromForm] Books book)
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");
        if (isAdmin == "False" || isAdmin == null)
        {
            return RedirectToAction("Login", "User");
        }

        if (id != book.BookID)
        {
            return Json(new { success = false, message = "ID mismatch" });
        }

        var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookID == id);
        if (existingBook == null)
        {
            return Json(new { success = false, message = "Book not found" });
        }

        // Handle discount and date logic
        if (book.Discount > 0)
        {
            if (existingBook.Discount == 0 || 
                existingBook.Discount != book.Discount || 
                existingBook.DiscountDate < DateTime.Now)
            {
                book.DiscountDate = DateTime.Now.AddDays(7);
            }
            else
            {
                book.DiscountDate = existingBook.DiscountDate;
            }
        }
        else
        {
            book.DiscountDate = DateTime.MinValue;
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        return Json(new { success = false });
    }
    
    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookID == id);
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteReview(int RevID)
    {
        try
        {
            var review = await _context.SiteReview.FindAsync(RevID);
            if (review == null)
            {
                return Json(new { success = false, message = "Review not found" });
            }

            _context.SiteReview.Remove(review);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            // Log the error
            return Json(new { success = false, message = ex.Message });
        }
    }
}