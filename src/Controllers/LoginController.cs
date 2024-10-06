using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.Patient;

namespace Sempi5.Controllers
{
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        
        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // TODO
        // Maybe change this method to see:
        //      1. Whether its his first login, give him is role 'Patient'
        //      2. If not, gave him his role
        //      3. Ask for username if its his first login
        

        [HttpGet("google-response")]
        [Authorize]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            var claimsIdentity = (ClaimsIdentity) result.Principal.Identity;

            // Patient role
            if (!claimsIdentity.FindAll(ClaimTypes.Role).Any(c => c.Value == "Patient"))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Patient"));
            }
            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            List<string> roles = claimsIdentity.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
           
            return Ok(new { success = true, email, roles, name });
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return Redirect("/Login/login");
        }

    }
}
