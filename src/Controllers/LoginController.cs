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
            var redirectUrl = Url.Content("/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("google-response")]
        [Authorize]
        public IActionResult GoogleResponse()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userClaims = claimsIdentity?.Claims.ToDictionary(c => c.Type, c => c.Value);

            return Ok(new { success = true, Claims = userClaims });
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
