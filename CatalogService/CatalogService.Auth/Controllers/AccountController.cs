
using CatalogService.Auth.Models;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatalogService.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(string email, string password, string returnUrl)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
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
               
                var roleExist = await _roleManager.RoleExistsAsync(role.ToString());
                if (!roleExist)
                {
                    var roleInDb = await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }

                var _ = await _userManager.AddToRoleAsync(currentUser, role.ToString());

                _ = await _userManager.AddClaimsAsync(user, new Claim[]{
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

            throw new Exception("Can't register new user");
        }

        


    }
}
