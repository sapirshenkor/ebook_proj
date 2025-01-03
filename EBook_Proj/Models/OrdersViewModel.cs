namespace EBook_Proj.Models;

public class OrdersViewModel
{
    public List<Orders> Orders { get; set; }
    public Dictionary<int, List<dynamic>> OrderDetails { get; set; }
}