
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using CatalogService.Auth.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CatalogService.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEventService _events;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEventService events,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _events = events;
            _logger = logger;
        }

        [HttpGet("login")]
        public async Task<IActionResult> LoginAsync(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("For user {user} login was succesful", email);
                var user = await _userManager.FindByNameAsync(email);
                var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                var claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Subject, user.Id),
                    new Claim(JwtClaimTypes.Name, user.UserName),
                    new Claim(JwtClaimTypes.Role, userRole)

                };

                // issue authentication cookie with subject ID and username
                var isuser = new IdentityServerUser(user.Id)
                {
                    DisplayName = user.UserName,
                    AdditionalClaims = claims
                };

                await HttpContext.SignInAsync(isuser);
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                var token = await HttpContext.GetTokenAsync("access_token");
                return Ok(token);
            }

            _logger.LogInformation("For user {user} login was unsuccesful", email);
            return Unauthorized();
        }

        [HttpGet("logout")]
        public async Task LogoutAsync()
        {
            if (HttpContext.User?.Identity?.Name != null)
            {
                _logger.LogInformation("User {user} login was logged out", HttpContext.User.Identity.Name);
                await _signInManager.SignOutAsync();
            }
        }

        [HttpGet]
        public async Task<IActionResult> RegisterAsync(string name, string password, Role role, string returnUrl)
        {
            var user = new IdentityUser { UserName = name, Email = name };
            var result = await _userManager.CreateAsync(user, password);
            _logger.LogInformation("User {name} was registered", name);

            if (result.Succeeded)
            {
                var currentUser = await _userManager.FindByNameAsync(name);

                var roleExist = await _roleManager.RoleExistsAsync(role.ToString());
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }

                await _userManager.AddToRoleAsync(currentUser, role.ToString());

                await _userManager.AddClaimsAsync(user, new Claim[]{
                            new Claim(JwtClaimTypes.Name, name),
                            new Claim(JwtClaimTypes.Role, role.ToString()),
                            new Claim(JwtClaimTypes.Subject, user.Id),
                        });


                await _signInManager.SignInAsync(currentUser, isPersistent: false);

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return Ok(JsonSerializer.Serialize(currentUser));

            }

            return BadRequest("Can't register new user");
        }
    }
}
