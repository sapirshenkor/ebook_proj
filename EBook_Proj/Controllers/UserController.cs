using EBook_Proj.Models;
using Microsoft.AspNetCore.Mvc;

namespace EBook_Proj.Controllers;

public class UserController : Controller
{
    // GET
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult UserPage(UserModel user)
    {
        
      //check validation
      if (ModelState.IsValid)
      {
          return View("UserPageView",user);
      }
      
      return View("CreateUserView", user);
    }

    public ActionResult CreateUser()
    {
        return View("CreateUserView", new UserModel());
    }
    
    
    
}