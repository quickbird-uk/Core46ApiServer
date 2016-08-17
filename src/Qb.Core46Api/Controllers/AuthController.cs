using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Qb.Core46Api.Helpers;
using Qb.Core46Api.Models;
using Qb.Core46Api.Services;

namespace Qb.Core46Api.Controllers
{
    /// <summary>Authorization API for use with the standard app flow.</summary>
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<QbUser> _userManager;

        public AuthController(UserManager<QbUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>Register new user using phone confirmation.</summary>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(string username, string password, string phonenumber,
            [FromServices] ISmsSender smsSender)
        {
            var pars = new[] {username, password, phonenumber};
            if (pars.Any(string.IsNullOrWhiteSpace))
                return Res.PlainUtf8(
                    "One or more of required fields missing or empty: username, password, phonenumber", 400);

            var user = new QbUser
            {
                UserName = username,
                PhoneNumberConfirmed = false
            };

            var res = await _userManager.CreateAsync(user, password);

            if (res.Succeeded)
            {
                user = await _userManager.FindByNameAsync(username);
                var phoneToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phonenumber);

                // Ignore used for manual admin verified user.
                if (phonenumber.ToLowerInvariant() != "ignore")
                    if (!await smsSender.SendSms($"QB sign-up code:{phoneToken}", phonenumber))
                        return
                            Res.PlainUtf8(
                                "User created but sms failed, try re-requesting code by changing phonenumber.", 400);

                return Res.PlainUtf8($"User {username} successfully created, needs verification.");
            }

            return Res.PlainUtf8(res.PrettyErrors(), 400);
        }


        /// <summary>Change phone number with text confirmation. Also use to re-send code.</summary>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangePhoneNumber(string username, string password, string phonenumber,
            [FromServices] ISmsSender smsSender)
        {
            var pars = new[] {username, phonenumber, password};
            if (pars.Any(string.IsNullOrWhiteSpace))
                return
                    Res.PlainUtf8(
                        "One or more of required fields username, phonenumber and password were missing or empty.", 400);

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Res.PlainUtf8("User does not exist", 400);

            // Need to do a manual check as the user cannot log in and authorize before confirming the phone number.
            var pass = await _userManager.CheckPasswordAsync(user, password);
            if (!pass)
                return Res.PlainUtf8("Invalid password", 400);

            var phoneToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phonenumber);
            if (!await smsSender.SendSms($"QB sign-up code:{phoneToken}", phonenumber))
                return Res.PlainUtf8("User created but sms failed, try re-requesting code by changing phonenumber.", 400);
            return Res.PlainUtf8($"New phone code texted for {username}.");
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmPhone(string username, string phonenumber, string code)
        {
            var pars = new[] {username, phonenumber, code};
            if (pars.Any(string.IsNullOrWhiteSpace))
                return
                    Res.PlainUtf8(
                        "One or more of required fields username, phonenumber and code were missing or empty.", 400);

            var user = await _userManager.FindByNameAsync(username);
            var valid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, code, phonenumber);
            if (!valid)
                return Res.PlainUtf8("Invalid Code", 400);
            user.PhoneNumber = phonenumber;
            user.PhoneNumberConfirmed = true;
            var res = await _userManager.UpdateAsync(user);
            return res.Succeeded
                ? Res.PlainUtf8("Success, phone number confirmed.")
                : Res.PlainUtf8(res.PrettyErrors(), 500);
        }
    }
}