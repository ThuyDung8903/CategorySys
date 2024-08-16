using CategorySys.DTO;
using CategorySys.Services;
using Microsoft.AspNetCore.Mvc;

namespace CategorySys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDTO userDto)
        {
            _authService.Register(userDto);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public IActionResult Login(UserDTO userDto)
        {
            var token = _authService.Login(userDto);

            return Ok(new { Token = token });
        }
    }
}
