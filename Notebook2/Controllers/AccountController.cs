using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Notebook2.Service;
using Notebook2.ViewModel;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Notebook2.Controllers
{
    public class AccountController : Controller
    {
        Repository repository = new Repository();
        public ActionResult Index()
        {
            //if (HttpContext.User.Identity.IsAuthenticated) //check if authenticated user is available
            //{
            //    return RedirectToAction("Index", "Home"); //if yes redirect to HomeController - means user is still log in
            //}

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

        public ActionResult Login() //Login
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model) //Login
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await repository.LoginData(model);
            if (result.resultCode == 800)
            {
                TempData["UserData"] = result.Data;
                dynamic obj = JsonConvert.DeserializeObject(result.Data);
                string Name = obj[0].name +" "+ obj[0].surname;

                 var claims = new List<Claim>
                 {
                        new Claim(ClaimTypes.Name, Name),
                        // добавление других идентификационных данных пользователя
                 };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                ModelState.AddModelError("", result.message);
            }

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}
