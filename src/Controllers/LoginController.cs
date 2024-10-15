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
    [ApiController]
    public class LoginController : ControllerBase
    {
         private readonly PatientService _service;

        public LoginController(PatientService service)
        {
            _service = service;
        }
        
        [HttpGet("asPatient")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Content("/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("register")]
        public IActionResult Register(PatientDTO patient)
        {
            // TODO: Implement registration logic
            //       Call patient service
        
            return Ok(_service.AddPatient(patient));
        }

        [HttpGet("google-response")]
        [Authorize]
        public async Task<IActionResult> GoogleResponse()
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
