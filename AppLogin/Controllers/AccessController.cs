using AppLogin.Data;
using AppLogin.Models;
using AppLogin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace AppLogin.Controllers
{
    public class AccessController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccessController(ApplicationDbContext DbContext)
        {
            _dbContext = DbContext;
        }

        //Register View
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserVM userModel)
        {
            if(userModel.Password != userModel.ConfirmPassword)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();           
            }

            User user = new User()
            {
                FullName = userModel.FullName,
                Email = userModel.Email,
                Password = userModel.Password
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            if(user.IdUser != 0) return RedirectToAction("Login", "Access");

            ViewData["Mensaje"] = "No se pudo crear el usuario";

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginModel)
        {
            User? user_found = await _dbContext.Users
                    .Where(u =>
                        u.Email == loginModel.Email &&
                        u.Password == loginModel.Password
                        ).FirstOrDefaultAsync();

            if (user_found is null) 
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias.";

                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user_found.FullName)
            };

            ClaimsIdentity? claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );

            return RedirectToAction("Index", "Home");
        }
    }
}
