using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Qb.Core46Api.Helpers;
using Qb.Core46Api.Models;

namespace Qb.Core46Api.Controllers
{
    /// <summary>Management and helper authorization api.</summary>
    [Route("api/[controller]")]
    public class AuthManageController : Controller
    {
        private readonly UserManager<QbUser> _userManager;

        public AuthManageController(UserManager<QbUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>Gets calims from current authorization (property of the token/signin, not a property of the the user).</summary>
        /// <returns>List of | separated calims.</returns>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public IActionResult MyClaims()
        {
            var claims = User.Claims.Select(c => $"Subject:{c.Subject}, Type:{c.Type}, Value:{c.Value}");
            var response = Res.PlainUtf8("Claims:" + string.Join("|", claims));
            return response;
        }

        /// <summary>Admin is the god role, never give to users.</summary>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GrantAdminRole(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var result = await _userManager.AddToRoleAsync(user, "admin");
            if (result.Succeeded)
                return Res.PlainUtf8($"Admin role granted to {username}.");

            var response = Res.PlainUtf8(result.PrettyErrors(), 400);
            return response;
        }

        /// <summary>Sets phone number for a user skipping text authorization.</summary>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetPhonenumberAndConfirm(string username, string phonenumber)
        {
            var user = await _userManager.FindByNameAsync(username);
            user.PhoneNumber = phonenumber;
            user.PhoneNumberConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Res.PlainUtf8($"{username} phone number set and confirmed.");

            var response = Res.PlainUtf8(result.PrettyErrors(), 400);
            response.StatusCode = 400;
            return response;
        }

        /// <summary>Test authorization for any user (404 on fail).</summary>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public IActionResult AmIAuthorized()
        {
            return Res.PlainUtf8("true");
        }

        /// <summary>Test if admin role authorization is granted (404 on fail).</summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("[action]")]
        public IActionResult AmIAuthorizedAdmin()
        {
            return Res.PlainUtf8("true");
        }


        /// <summary>Gets a phone auth token - for testing.</summary>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetPhoneTokenDirect(string username, string phonenumber)
        {
            var pars = new[] {username, phonenumber};
            if (pars.Any(string.IsNullOrWhiteSpace))
                return
                    Res.PlainUtf8(
                        "One or more of required fields username, phonenumber and password were missing or empty.", 400);

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Res.PlainUtf8("User does not exist", 400);

            var phoneToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phonenumber);

            return Res.PlainUtf8(phoneToken);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return Res.PlainUtf8("Username missing or blank.", 400);

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Res.PlainUtf8("User does not exist", 400);

            var res = await _userManager.DeleteAsync(user);

            if (res.Succeeded)
                return Res.PlainUtf8($"User \"{username}\" deleted.");
            return Res.PlainUtf8(res.PrettyErrors(), 400);
        }
    }
}