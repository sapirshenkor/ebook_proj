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

    public IActionResult AdminPage()
    {
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> ShowUsers()
    {
        var AllUsers =await _context.Users.ToListAsync();
        return View(AllUsers);
    }

    [HttpGet]
    public async Task<IActionResult> ShowBooks()
    {
        var AllBooks =await _context.Books.ToListAsync();
        return View(AllBooks);
    }

    [HttpGet]
    public async Task<IActionResult> ShowReviews()
    {
        var allRev = await _context.SiteReview.ToListAsync();
        return View(allRev);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromForm] Books book)
    {
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
        if (id != book.BookID)
        {
            return Json(new { success = false, message = "ID mismatch" });
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.BookID))
                    return Json(new { success = false, message = "Book not found" });
                throw;
            }
        }
        return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
    }
    
    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.BookID == id);
    }
}