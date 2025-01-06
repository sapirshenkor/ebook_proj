using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using EBook_Proj.DATA;
using EBook_Proj.Models;
using EBook_Proj.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;


namespace EBook_Proj.Controllers;

public class Payment: Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public Payment(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
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
        if (HttpContext.Session.GetString("CustomerID") == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        int customerId = int.Parse(HttpContext.Session.GetString("CustomerID"));
        var currentDate = DateTime.Now;
        decimal total = decimal.Parse(HttpContext.Session.GetString("CartTotal"));
        string userEmail = HttpContext.Session.GetString("Email");
        string firstName = HttpContext.Session.GetString("FirstName");
        string lastName = HttpContext.Session.GetString("LastName");

        var order = new Orders
        {
            CustomerID = customerId,
            TotalAmount = total,
            PurchaseDate = currentDate
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        // Prepare email content
        StringBuilder bookList = new StringBuilder();
        foreach (var item in cartItems)
        {
            var book = await _context.Books.FindAsync(item.BookId);
            string accessType = item.Type == "buy" ? "Purchased" : "Borrowed";
            bookList.AppendLine($"<tr>");
            bookList.AppendLine($"<td>{book.Title}</td>");
            bookList.AppendLine($"<td>{book.Author}</td>");
            bookList.AppendLine($"<td>{accessType}</td>");
            bookList.AppendLine($"<td>${item.Price}</td>");
            bookList.AppendLine($"</tr>");
            
            var orderdetails = new OrderDetails
            {
                OrderID = order.OrderID,
                BookID = item.BookId,
                BookPrice = item.Price,
                BookType = item.Type
            };
            var booksUser = new BooksUserModel()
            {
                BookID = item.BookId,
                UserID = customerId,
                Type = item.Type,
                Date = currentDate
            };
            if (item.Type == "borrow")
            {
                var BorrowedBooks = await _context.Books.FirstOrDefaultAsync(b => b.BookID == item.BookId);
                BorrowedBooks.BorrowCount -= 1;
                await _context.SaveChangesAsync();
            }
            _context.OrderDetails.Add(orderdetails);
            _context.BooksUser.Add(booksUser);
        }
        await _context.SaveChangesAsync();

        // Send order confirmation email
        string subject = "Your EBook Store Order Confirmation";
        string body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Thank you for your order, {firstName} {lastName}!</h2>
                <p>We're excited to confirm your order #{order.OrderID}.</p>
                
                <div style='margin: 20px 0;'>
                    <h3>Order Details:</h3>
                    <p>Order Date: {currentDate:MMM dd, yyyy HH:mm}</p>
                    <p>Total Amount: ${total}</p>
                    
                    <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                        <tr style='background-color: #f8f8f8;'>
                            <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Title</th>
                            <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Author</th>
                            <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Type</th>
                            <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Price</th>
                        </tr>
                        {bookList}
                    </table>
                </div>

                <p>You can access your books through your account on our website.</p>
                
                <p>If you borrowed any books, please remember that they will be available for 30 days.</p>
                <p>You will get a reminder notification 5 before the return.</p>

                <p>If you have any questions about your order, please don't hesitate to contact our support team.</p>

                <p>Happy Reading!</p>
                <br>
                <p>Best regards,</p>
                <p>The EBook Store Team</p>
            </body>
            </html>";

        try
        {
            await _emailService.SendEmailAsync(userEmail, subject, body, isHtml: true);
        }
        catch (Exception ex)
        {
            // Log the error but don't stop the order process
            Console.WriteLine($"Failed to send order confirmation email: {ex.Message}");
        }

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
            if (HttpContext.Session.GetString("CustomerID") == null)
            {
                return RedirectToAction("Login", "User");
            }
            
            int customerId = int.Parse(HttpContext.Session.GetString("CustomerID"));
            var currentDate = DateTime.Now;
            decimal total = decimal.Parse(HttpContext.Session.GetString("CartTotal"));
            string userEmail = HttpContext.Session.GetString("Email");
            string firstName = HttpContext.Session.GetString("FirstName");
            string lastName = HttpContext.Session.GetString("LastName");

            var order = new Orders
            {
                CustomerID = customerId,
                TotalAmount = total,
                PurchaseDate = currentDate
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Prepare email content
            string bookList = "";
            foreach (var item in cartItems)
            {
                var book = await _context.Books.FindAsync(item.BookId);
                string accessType = item.Type == "buy" ? "Purchased" : "Borrowed";
                bookList += $"<tr>";
                bookList += $"<td>{book.Title}</td>";
                bookList += $"<td>{book.Author}</td>";
                bookList += $"<td>{accessType}</td>";
                bookList += $"<td>${item.Price}</td>";
                bookList += $"</tr>";

                var orderdetails = new OrderDetails
                {
                    OrderID = order.OrderID,
                    BookID = item.BookId,
                    BookPrice = item.Price,
                    BookType = item.Type
                };
                var booksUser = new BooksUserModel()
                {
                    BookID = item.BookId,
                    UserID = customerId,
                    Type = item.Type,
                    Date = currentDate
                };
                _context.OrderDetails.Add(orderdetails);
                _context.BooksUser.Add(booksUser);
            }
            await _context.SaveChangesAsync();

            // Send order confirmation email
            string subject = "Your EBook Store PayPal Order Confirmation";
            string body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Thank you for your PayPal order, {firstName} {lastName}!</h2>
                    <p>We're excited to confirm your order #{order.OrderID}.</p>
                    
                    <div style='margin: 20px 0;'>
                        <h3>Order Details:</h3>
                        <p>Order Date: {currentDate:MMM dd, yyyy HH:mm}</p>
                        <p>Total Amount: ${total}</p>
                        <p>Payment Method: PayPal</p>
                        
                        <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                            <tr style='background-color: #f8f8f8;'>
                                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Title</th>
                                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Author</th>
                                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Type</th>
                                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Price</th>
                            </tr>
                            {bookList}
                        </table>
                    </div>

                    <p>You can access your books through your account on our website.</p>
                    
                    <p>If you borrowed any books, please remember that they will be available for 30 days.</p>

                    <p>If you have any questions about your order, please don't hesitate to contact our support team.</p>

                    <p>Happy Reading!</p>
                    <br>
                    <p>Best regards,</p>
                    <p>The EBook Store Team</p>
                </body>
                </html>";

            try
            {
                await _emailService.SendEmailAsync(userEmail, subject, body, isHtml: true);
            }
            catch (Exception ex)
            {
                // Log the error but don't stop the order process
                Console.WriteLine($"Failed to send order confirmation email: {ex.Message}");
            }

            ViewBag.CartTotal = HttpContext.Session.GetString("CartTotal");
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error processing order" });
        }
    }
}