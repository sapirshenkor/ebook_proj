using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EBook_Proj.Models;

public class Orders
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderID { get; set; }
    public int CustomerID { get; set; }
    public decimal TotalAmount { get; set; }

    public DateTime PurchaseDate { get; set; }

    public virtual UserModel Customer { get; set; }
    public virtual ICollection<OrderDetails> OrderDetails { get; set; }
}