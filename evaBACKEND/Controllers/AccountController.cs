using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using evaBACKEND.Data;
using evaBACKEND.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace evaBACKEND.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager, 
			SignInManager<AppUser> signInManager,
			RoleManager<IdentityRole> roleManager,
			IConfiguration configuration, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
			_roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [Route("register")]
		[Authorize(AuthenticationSchemes = "Bearer", Roles ="Admin")]
        [HttpPost]
		public async Task<IActionResult> CreateUserAsync([FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("User model is invalid");
            }
            var user = new AppUser { UserName = model.Email, Email = model.Email,
				FirstName = model.FirstName, LastName = model.LastName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
				await _userManager.AddToRoleAsync(user, model.Role);
				return await BuildTokenAsync(model);
            }
            return BadRequest($"User with email: {model.Email} already exist.!");
        }

        [AllowAnonymous]
        [Route("account/login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
			IActionResult response = Unauthorized();

			if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User doesn't exist" });
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }
			var userPrincipal = new UserModel();
			userPrincipal.Email = model.Email;
			userPrincipal.Password = model.Password;
            return await BuildTokenAsync(userPrincipal);
        }

        [HttpGet]
        [Route("users")]
        public IEnumerable<AppUser> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        [HttpGet]
        [Route("users/{id}")]
        public IActionResult GetUserById([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        private async Task<IActionResult> BuildTokenAsync(UserModel model)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var principal = await _userManager.FindByEmailAsync(model.Email);
			var rolesByUser = await _userManager.GetRolesAsync(principal);

            var expiration = DateTime.UtcNow.AddHours(1);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, model.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("roles", rolesByUser.First()),
				new Claim("id", principal.Id),
			};

			JwtSecurityToken token = new JwtSecurityToken(
			   issuer: _configuration["Jwt:Issuer"],
			   audience: _configuration["Jwt:Issuer"],
			   claims: claims,
			   expires: expiration,
			   signingCredentials: creds
			);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });

        }
    }
}
