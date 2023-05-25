using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Notebook2.Service;
using Notebook2.ViewModel;
using System.Security.Claims;

namespace Notebook2.Controllers
{
    public class AccountController : Controller
    {
        Repository repository = new Repository();
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated) //check if authenticated user is available
            {
                return RedirectToAction("Index", "Home"); //if yes redirect to HomeController - means user is still log in
            }

            return View();
        }
        public ActionResult Register() //Register
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model) //Register
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var result = await repository.RegisterData(model);
            if (result.resultCode == 201)
            {
                return Redirect("/"); //redirect to login form
            }
            else
            {
                ModelState.AddModelError("", result.message);
            }

            return View();
        }
    }
}
