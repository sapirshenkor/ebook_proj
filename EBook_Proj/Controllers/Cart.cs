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
    public IActionResult Checkout([FromBody] List<CartItemModel> cartItems)
    {
        try 
        {
            // Store cart in session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cartItems));
        
            // Calculate and store total
            //decimal total = cartItems.Sum(item => item.Price * item.Quantity);
            decimal sum = 0;
            foreach (var item in cartItems)
            {
                sum+=item.Price * item.Quantity;
            }
            HttpContext.Session.SetString("CartTotal", sum.ToString());
        
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    // {
    //     try
    //     {
    //         //check if the user is loged in
    //         if (HttpContext.Session.GetString("CustomerID") == null)
    //         {
    //             return RedirectToAction("Login", "User");
    //         }
    //         int customerId=int.Parse(HttpContext.Session.GetString("CustomerID"));
    //         
    //         //entering the first book from the cart to create the order id
    //         var firstBook = new Orders
    //         {
    //             CustomerID = customerId,
    //             BookID = cartItems[0].BookId,
    //             BookPrice = (int)cartItems[0].Price,
    //             BookType = cartItems[0].Type
    //         };
    //         _context.Orders.Add(firstBook);
    //         _context.SaveChanges();
    //         
    //         //we wil use the same OrderID for the remaining Books
    //         for (int i = 1; i < cartItems.Count; i++)
    //         {
    //             var restOrder = new Orders
    //             {
    //                 OrderID = firstBook.OrderID,
    //                 CustomerID = customerId,
    //                 BookID = cartItems[i].BookId,
    //                 BookPrice = (int)cartItems[i].Price,
    //                 BookType = cartItems[i].Type
    //             };
    //             _context.Orders.Add(restOrder);
    //         }
    //         _context.SaveChanges();
    //         
    //     }
    // }
    
}