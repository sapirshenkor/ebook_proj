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
            if (user != null)
            {
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("CustomerID", user.CustomerID.ToString());
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                return RedirectToAction("UserPage", "User");
            }
            ModelState.AddModelError("", "Invalid email or password");
        }
        return View();
    }

    public async Task<IActionResult> SiteReview(SiteReviewModel siteReview)
    {
        if (ModelState.IsValid)
        {
            var customerIDString = HttpContext.Session.GetString("CustomerID");
            if (string.IsNullOrEmpty(customerIDString))
            {
                return RedirectToAction("Login", "User");

            }
            siteReview.UserID = int.Parse(customerIDString);
            _context.Add(siteReview);
            await _context.SaveChangesAsync();
            return RedirectToAction("UserPage", "User");
        }
        return View("UserPage");
        
    }

    public IActionResult UserPage()
    {
        return View();
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
}