using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.PatientEntity;

namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Content("api/login/redirect-to-frontend");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("redirect-to-frontend")]
        public IActionResult RedirectToFrontEnd()
        {   var claimsIdentity = User.Identity as ClaimsIdentity;
            var role = claimsIdentity?.FindFirst(ClaimTypes.Role)?.Value;
            switch (role?.ToLower()){
                case "patient":
                    return Redirect("http://localhost:4200/patient");
                case "admin":
                    return Redirect("http://localhost:4200/admin");
                case "doctor":
                case "other":
                    return Redirect("http://localhost:4200/staff");
                default:
                    return Redirect("http://localhost:4200");
            }
        }


        [HttpGet("google-response")]
        [Authorize]
        public IActionResult GoogleResponse()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userClaims = new
            {
                name = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value,
                email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value,
                role = claimsIdentity?.FindFirst(ClaimTypes.Role)?.Value
            };

            return Ok(new { userClaims });
        }


        [HttpGet("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return Ok(new { success = true});
        }

    }
}
