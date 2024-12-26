using Microsoft.AspNetCore.Mvc;

namespace EBook_Proj.Controllers;

public class Cart : Controller
{
    // GET
    public IActionResult ShowCart()
    {
        return View();
    }
}