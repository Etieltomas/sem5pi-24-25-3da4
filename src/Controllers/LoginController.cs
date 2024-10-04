using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sempi5.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        [Authorize]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var birthdate = claims?.FirstOrDefault(x => x.Type == ClaimTypes.DateOfBirth)?.Value;
            var name = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var picture = claims?.FirstOrDefault(x => x.Type == "picture")?.Value;
            var country = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Country)?.Value;      

            return Json(new { success = true, email , birthdate, name, picture, country });
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            // Signs out the user from the authentication scheme
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Redirects to the home page or login page after sign out
            return Redirect("/");
        }

    }
}
