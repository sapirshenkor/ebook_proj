using System.Text.Json;
using EBook_Proj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.DATA;


namespace EBook_Proj.Controllers;

public class Cart : Controller
{
    private readonly ApplicationDbContext _context;

    public Cart(ApplicationDbContext context)
    {
        _context = context;
    }
    // GET
    public IActionResult ShowCart()
    {
        return View();
    }

    [HttpPost]
    public async Task <IActionResult> Checkout([FromBody] List<CartItemModel> cartItems)
    {
        try 
        {
            var allBooks = await _context.BooksUser
                .Where(b => b.UserID == int.Parse(HttpContext.Session.GetString("CustomerID")))
                .ToListAsync();
            
            var bookIDBorrow = allBooks
                .Where(od => od.Type == "borrow")
                .Select(od => od.BookID)
                .ToList();
            var borrowedBooksInCart = 0;
            decimal sum = 0;
            // Calculate and store total
            //decimal total = cartItems.Sum(item => item.Price * item.Quantity);
            foreach (var item in cartItems)
            {
                sum+=item.Price * item.Quantity;
                if (item.Type == "borrow")
                {
                    borrowedBooksInCart++;
                }
            }
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
            if (bookIDBorrow.Count >=3 && borrowedBooksInCart != 0 )
            {
                return Json(new { success = false, message = "You can't have more than 3 borrowed books" });
            }
            HttpContext.Session.SetString("CartTotal", sum.ToString());
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    
}