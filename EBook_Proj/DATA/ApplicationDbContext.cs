using System.Security.AccessControl;
using EBook_Proj.Models;

namespace EBook_Proj.DATA;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {}
    public DbSet<UserModel> Users { get; set; }

    public DbSet<Books> Books { get; set; }

    public DbSet<SiteReviewModel> SiteReview {get; set;}
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
}

