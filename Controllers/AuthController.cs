using CategorySys.DTO;
using CategorySys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CategorySys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly JwtOption _jwtOption;

        public AuthController(AppDbContext context, IOptions<JwtOption> options)
        {
            _context = context;
            _jwtOption = options.Value;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDTO userDto)
        {
            if (_context.Users.Any(u => u.UserName == userDto.UserName))
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                UserName = userDto.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully");
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("_Login");
        }

        [HttpPost("login")]
        public IActionResult Login(UserDTO userDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == userDto.UserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOption.Issuer,
                audience: _jwtOption.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
