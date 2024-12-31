using Microsoft.EntityFrameworkCore;

namespace EBook_Proj.Models;

[PrimaryKey(nameof(OrderID), nameof(BookID))]

public class OrderDetails
{
    public int OrderID { get; set; }
    public int BookID { get; set; }
    public decimal BookPrice { get; set; }
    public string BookType { get; set; }

    public virtual Orders Order { get; set; }
    public virtual Books Book { get; set; }
}