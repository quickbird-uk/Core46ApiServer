using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict;
using Qb.Core46Api.Helpers;
using Qb.Core46Api.Models;
using Qb.Core46Api.Services;
using Qb.Poco;

namespace Qb.Core46Api.Controllers
{
    /// <summary>Authorization API for use with the standard app flow.</summary>
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly SignInManager<QbUser> _signInManager;
        private readonly OpenIddictUserManager<QbUser> _userManager;

        public AuthController(OpenIddictUserManager<QbUser> userManager, SignInManager<QbUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("/api/auth/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIdConnectRequest();

            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                    return Res.JsonErrorResult("invalid", 401);

                // Chack that email or phone verification is done if required.
                if (!await _signInManager.CanSignInAsync(user))
                    return Res.JsonErrorResult("needs_confirm", 401); // Actual response is 302 redirect.

                // Reject the token request if two-factor authentication has been enabled by the user.
                if (_userManager.SupportsUserTwoFactor && await _userManager.GetTwoFactorEnabledAsync(user))
                    return Res.JsonErrorResult("2fa", 401);

                // Lockout response must be the same as invalid credentials to foil brute force attacks.
                if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                    return Res.JsonErrorResult("invalid", 401);

                // Ensure the password is valid.
                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    if (_userManager.SupportsUserLockout)
                        await _userManager.AccessFailedAsync(user);

                    return Res.JsonErrorResult("invalid", 401);
                }

                if (_userManager.SupportsUserLockout)
                    await _userManager.ResetAccessFailedCountAsync(user);

                var identity = await _userManager.CreateIdentityAsync(user, request.GetScopes());

                // Create a new authentication ticket holding the user identity.
                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties(),
                    OpenIdConnectServerDefaults.AuthenticationScheme);

                ticket.SetResources(request.GetResources());
                ticket.SetScopes(request.GetScopes());

                return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }

            return BadRequest(new OpenIdConnectResponse
            {
                Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }

        /// <summary>Register new user using phone confirmation.</summary>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(string username, string password, string phonenumber,
            [FromServices] ISmsSender smsSender, [FromServices] QbDbContext db)
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
                // Creates a full user with all edit abilities for own information.
                var userGuid = new Guid(user.Id);

                // Create a Person with the same guid as the identity user.
                await CreatePerson(userGuid, db);

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

        /// <summary>Creates a Person in the DB. The Person object has no knowledge of ASP.Net Identity. The Person's Guid will be
        ///     the same as the ID of the IdentityUser's Id, which is a Guid stored as a string.</summary>
        /// <param name="userGuid">The guid that the identity system will use to refer to this Person.</param>
        /// <param name="db">The databse context.</param>
        /// <returns>Awaitable, waiting the save on the database.</returns>
        private async Task CreatePerson(Guid userGuid, QbDbContext db)
        {
            // The update and create dates are auto-initialised.
            var person = new Person
            {
                Id = userGuid
            };
            db.People.Add(person);
            await db.SaveChangesAsync();
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