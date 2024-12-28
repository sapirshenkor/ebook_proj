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
    // GET
    public IActionResult AdminPage()
    {
        return View();
    }

    public IActionResult ShowUsers()
    {
        var AllUsers = _context.Users.ToList();
        return View(AllUsers);
    }

    public IActionResult ShowBooks()
    {
        var AllBooks = _context.Books.ToList();
        return View(AllBooks);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(Books book)
    {
        if (ModelState.IsValid)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowBooks");
        }
        return RedirectToAction("AdminPage");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBook(Books book)
    {
        var deleteBook = await _context.Books.FindAsync(book.BookID);
        if (deleteBook == null)
        {
            return NotFound();
        }
        
        _context.Books.Remove(deleteBook);
        await _context.SaveChangesAsync();
        return RedirectToAction("ShowBooks");
    }
    
    
    
   
}