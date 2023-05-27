using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Notebook2.Service;
using Notebook2.ViewModel;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Notebook2.Controllers
{
    public class AccountController : Controller
    {
        Repository repository = new Repository();
        // Return Home page.
        public ActionResult Index()
        {
            return View();
        }

        //Return Register view
        public ActionResult Register()
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
                string Name = obj[0].name + obj[0].surname;

                //var claims = new List<Claim>
                //{
                //new Claim(ClaimTypes.Name, Name)
                //};

                //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //var principal = new ClaimsPrincipal(identity);

                //HttpContext.SignInAsync(principal).Wait();

                return Redirect("/"); //redirect to login form
            }
            else
            {
                ModelState.AddModelError("", result.message);
            }

            return View();
        }
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return Redirect("/");
        }

    }
}
