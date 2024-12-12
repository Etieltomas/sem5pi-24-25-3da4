using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Sempi5.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class LoginController : ControllerBase 
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject IConfiguration
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Content("api/login/redirect-to-frontend");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("redirect-to-frontend")]
        public IActionResult RedirectToFrontEnd()
        {
            // Retrieve the FrontEnd URL from the configuration
            var frontEndUrl = _configuration["IpAddresses:FrontEnd"] ?? "http://localhost:4200"; // Default to localhost:4200 if not configured

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var role = claimsIdentity?.FindFirst(ClaimTypes.Role)?.Value;
            var email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = false, 
                SameSite = SameSiteMode.Unspecified,
                Expires = DateTime.UtcNow.AddMinutes(30),
                Domain = ".sarm.com"
            };

            // Set user info in cookies
            Response.Cookies.Append("UserInfo", $"role={role}&email={email}", cookieOptions);

            // Redirect to the frontend URL
            return Redirect(frontEndUrl);
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
            Request.Cookies.TryGetValue("UserInfo", out var userInfo);
            if (userInfo != null)
            {
                Response.Cookies.Delete("UserInfo");
            }
            return Ok(new { success = true });
        }
    }
}
