using System.Security.AccessControl;
using EBook_Proj.Models;

namespace EBook_Proj.DATA;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {}
    public DbSet<UserModel> Users { get; set; }
}