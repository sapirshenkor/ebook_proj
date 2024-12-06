using EBook_Proj.Models;
using Microsoft.AspNetCore.Mvc;

namespace EBook_Proj.Controllers;

public class UserController : Controller
{
    // GET
    public ActionResult UserPage()
    {
        var user = new UserModel() {Name = "Liran", LastName = "Hozias", Email = "hliran2@gmail.com", Username = "hliran2"};
        return View(user);
      // //check validation
      // if (ModelState.IsValid)
      // {
      //     return View("UserPageView",user);
      // }
      //
      // return View("CreateUserView", user);
    }

    public ActionResult CreateUser()
    {
        return View("CreateUserView", new UserModel());
    }
    
    
    
}