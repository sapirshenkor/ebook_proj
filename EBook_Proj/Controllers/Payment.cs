using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using EBook_Proj.DATA;
using EBook_Proj.Models;


namespace EBook_Proj.Controllers;

public class Payment: Controller
{
    private readonly ApplicationDbContext _context;

    public Payment(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Select()
    {
        // Check if user is logged in
        if (HttpContext.Session.GetString("CustomerID") == null)
        {
            return RedirectToAction("Login", "User");
        }
        ViewBag.CartTotal = HttpContext.Session.GetString("CartTotal");
        
        return View();
    }
    public IActionResult PayWithCard()
    {
        if (HttpContext.Session.GetString("CustomerID") == null)
        {
            return RedirectToAction("Login", "User");
        }

        ViewBag.CartTotal = HttpContext.Session.GetString("CartTotal");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ProcessCardPayment([FromBody] List<CartItemModel> cartItems)
    {
        try
        {
            //check if the user is logged in
            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Login", "User");
            }
            int customerId=int.Parse(HttpContext.Session.GetString("CustomerID"));
            var currentDate = DateTime.Now;  // Get current date/time
            decimal total = decimal.Parse(HttpContext.Session.GetString("CartTotal"));
            
            //entering the first book from the cart to create the order id
            var order = new Orders
            {
                CustomerID = customerId,
                TotalAmount = total,
                PurchaseDate = currentDate  // Set the date explicitly
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach (var item in cartItems)
            {
                var orderdetails = new OrderDetails
                {
                    OrderID = order.OrderID,
                    BookID = item.BookId,
                    BookPrice = item.Price,
                    BookType = item.Type
                };
                var booksUser = new BooksUserModel()
                {
                    BookId = item.BookId,
                    UserId = int.Parse(HttpContext.Session.GetString("CustomerID")),
                    Type = item.Type,
                    Date = currentDate
                };
                _context.OrderDetails.Add(orderdetails);
                _context.BooksUser.Add(booksUser);
            }
            _context.SaveChanges();
            ViewBag.CartTotal = HttpContext.Session.GetString("CartTotal");

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error processing order" });
        }
    }
    public IActionResult PaymentSuccess()
    {
        //check if the user is logged in
        if (HttpContext.Session.GetString("CustomerID") == null)
        {
            return RedirectToAction("Login", "User");
        }
        ViewBag.CartTotal = HttpContext.Session.GetString("CartTotal");
        return View();
    }

    public IActionResult PayWithPayPal()
    {
        //check if the user is logged in
        if (HttpContext.Session.GetString("CustomerID") == null)
        {
            return RedirectToAction("Login", "User");
        }
        ViewBag.CartTotal = HttpContext.Session.GetString("CartTotal");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayPalPayment([FromBody] List<CartItemModel> cartItems)
    {
        try
        {
            //check if the user is logged in
            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Login", "User");
            }
            int customerId=int.Parse(HttpContext.Session.GetString("CustomerID"));
            var currentDate = DateTime.Now;  // Get current date/time
            decimal total = decimal.Parse(HttpContext.Session.GetString("CartTotal"));
            
            //entering the first book from the cart to create the order id
            var order = new Orders
            {
                CustomerID = customerId,
                TotalAmount = total,
                PurchaseDate = currentDate  // Set the date explicitly
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach (var item in cartItems)
            {
                var orderdetails = new OrderDetails
                {
                    OrderID = order.OrderID,
                    BookID = item.BookId,
                    BookPrice = item.Price,
                    BookType = item.Type
                };
                var booksUser = new BooksUserModel()
                {
                    BookId = item.BookId,
                    UserId = int.Parse(HttpContext.Session.GetString("CustomerID")),
                    Type = item.Type,
                    Date = currentDate
                };
                _context.OrderDetails.Add(orderdetails);
                _context.BooksUser.Add(booksUser);
            }
            _context.SaveChanges();
            ViewBag.CartTotal = HttpContext.Session.GetString("CartTotal");

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error processing order" });
        }
    }
}