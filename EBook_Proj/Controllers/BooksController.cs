using System.Net;
using EBook_Proj.DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBook_Proj.Models;

namespace EBook_Proj.Controllers;

public class BooksController: Controller
{
    //this field holds the dataBase connection
    private readonly ApplicationDbContext _context;
    
    //constructor = gets database context through dependency injection
    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }
    // Get all books and resent them in the books catalog
    public async Task<IActionResult> BooksCatalog(string searchString, string genre, string priceSort, 
        string yearSort, string author, decimal? minPrice, decimal? maxPrice, string sortOption,string options)
    {
        // First get the IDs of popular books (top 5 most ordered)
        var popularBookIds = await _context.OrderDetails
            .GroupBy(od => od.BookID)
            .Select(group => new
            {
                BookID = group.Key,
                OrderCount = group.Count()
            })
            .OrderByDescending(x => x.OrderCount)
            .Take(5)
            .Select(x => x.BookID)
            .ToListAsync();

        var query = _context.Books.AsQueryable();

        // Apply all filters to the main query
        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(b => 
                b.Title.Contains(searchString) || 
                b.Author.Contains(searchString) || 
                b.Description.Contains(searchString));
        }
        
        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(b => b.Genre == genre);
        }

        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(b => b.Author == author);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(b => b.BuyingPrice >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(b => b.BuyingPrice <= maxPrice.Value);
        }

        IEnumerable<Books> popularBooks = new List<Books>();
        IEnumerable<Books> otherBooks = new List<Books>();

        // Get all filtered books
        var filteredBooks = await query.ToListAsync();

        if (sortOption == "popular")
        {
            // For popular sort, show popular books at top
            popularBooks = filteredBooks
                .Where(b => popularBookIds.Contains(b.BookID))
                .OrderBy(b => popularBookIds.IndexOf(b.BookID)); // Maintain popularity order

            otherBooks = filteredBooks
                .Where(b => !popularBookIds.Contains(b.BookID));

            // Apply secondary sorting to other books if needed
            otherBooks = ApplySecondarySort(otherBooks, priceSort, yearSort,options,popularBookIds);
        }
        else
        {
            // For other sorts, apply the sorting to all books
            otherBooks = ApplySecondarySort(filteredBooks, priceSort, yearSort,options,popularBookIds);
        }

        var viewModel = new HomePageBooksViewModel
        {
            PopularBooks = popularBooks,
            FeaturedBooks = otherBooks,
            PopularBookIds = popularBookIds, // Add this for consistent popular book identification
            Genres = await _context.Books.Select(b => b.Genre).Distinct().ToListAsync(),
            Authors = await _context.Books.Select(b => b.Author).Distinct().ToListAsync(),
            SelectedAuthor = author,
            SearchString = searchString,
            SelectedGenre = genre,
            SortOption = sortOption
        };

        return View(viewModel);
    }

    private IEnumerable<Books> ApplySecondarySort(IEnumerable<Books> books, string priceSort, string yearSort,string options,List<int> popularbookIds)
    {
        // Apply price sorting
        switch (priceSort?.ToLower())
        {
            case "asc":
                books = books.OrderBy(b => b.BuyingPrice);
                break;
            case "desc":
                books = books.OrderByDescending(b => b.BuyingPrice);
                break;
        }

        // Apply year sorting
        switch (yearSort?.ToLower())
        {
            case "asc":
                books = books.OrderBy(b => b.PublicationDate);
                break;
            case "desc":
                books = books.OrderByDescending(b => b.PublicationDate);
                break;
        }

        switch (options)
        {
            case "OnSale":
                books=books.Where(b => b.Discount > 0);
                break;
            case "ForBorrow":
                books=books.Where(b => !popularbookIds.Contains(b.BookID));
                break;
            case "ForBuying":
                books = books.ToList();
                break;
        }

        return books;
    }
       

    public async Task<IActionResult> BookDetails(int id)
    {
        
        
        var reviews = await _context.BookReview
            .Include(r => r.User)  // Include the User navigation property
            .Where(r => r.BookID == id)
            .ToListAsync();
        var book= await _context.Books.FirstOrDefaultAsync(b=>b.BookID == id);
        List<BooksUserModel> booksUserModels = new List<BooksUserModel>();
        if (book == null)
        {
            return NotFound();
        }
        List<BooksUserModel> booksusers = new List<BooksUserModel>();

        if (HttpContext.Session.GetString("CustomerID") != null)
        {
            var userIdString = HttpContext.Session.GetString("CustomerID");
            if (!string.IsNullOrEmpty(userIdString))
            {
                var customerID = int.Parse(userIdString);
                booksusers = await _context.BooksUser
                    .Where(b => b.UserID == customerID)
                    .ToListAsync();
            }
        }

        var bookdetails = new BookDetailsModal
        {
            Reviews = reviews,
            Book = book,
            BooksUser = booksusers
        };
        return View(bookdetails);

        
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
    public async Task<IActionResult> AddReview(BookReview review)
    {
        ModelState.Remove("User");

        if (ModelState.IsValid)
        {
            review.UserID = int.Parse(HttpContext.Session.GetString("CustomerID"));
            _context.BookReview.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BookDetails), new { id = review.BookID });
        }
        return RedirectToAction(nameof(BookDetails), new { id = review.BookID });
    }

    [HttpPost]
    public async Task<IActionResult> AddToWaitingList(int id)
    {
        var userID = HttpContext.Session.GetString("CustomerID");
        if (string.IsNullOrEmpty(userID))
        {
            return Json(new { success = false, redirect = "/User/Login" });
        }
    
        var bookInUser = await _context.BooksUser.FirstOrDefaultAsync(bu => bu.BookID == id && bu.UserID == int.Parse(userID));
        if (bookInUser != null)
        {
            return Json(new { success = false, message = "You already own this book!" });
        }

        var bookInWaitingList = await _context.WaitingList.FirstOrDefaultAsync(w => w.BookID == id && w.UserID == int.Parse(userID));
        if (bookInWaitingList == null)
        {
            var currentDate = DateTime.Now;
            var email = HttpContext.Session.GetString("Email");
            var waitingList = new WaitingListModel()
            {
                BookID = id,
                UserID = int.Parse(userID),
                Email = email,
                Date = currentDate
            };
            await _context.WaitingList.AddAsync(waitingList);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    
        return Json(new { success = false, message = "You are already in the waiting list for this book." });
    }
    
    
}
