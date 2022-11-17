
using CatalogService.Auth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CatalogService.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(string email, string password, string returnUrl)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            return Unauthorized();
        }

        [HttpGet("logout")]
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterAsync(string name, string password, Role role, string returnUrl)
        {
            var user = new IdentityUser { UserName = name, Email = name };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var currentUser = await _userManager.FindByNameAsync(name);
                var _ = await _userManager.AddToRoleAsync(currentUser, role.ToString());

                _ = await _userManager.AddClaimsAsync(user, new Claim[]{
                            new Claim("email", name),
                            new Claim("role", role.ToString())
                        });


                await _signInManager.SignInAsync(currentUser, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            throw new Exception("Can't register new user");
        }


    }
}
