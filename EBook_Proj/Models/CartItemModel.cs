namespace EBook_Proj.Models;

public class CartItemModel
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Cover { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; }
}