using Microsoft.AspNetCore.Mvc;
using ISLE.Interfaces;

namespace ISLE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (_userService.EmailExists(request.Email))
                return BadRequest("Email already exists.");

            var success = _userService.Register(request.UserName, request.Email, request.Password);
            if (!success)
                return StatusCode(500, "Failed to register user.");

            return Ok(new{ message = "User registered successfully."});
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _userService.Login(request.Email, request.Password);
            if(user == null){
                return Unauthorized(new { message="帳號或密碼錯誤"});
            }
            return Ok(new { message = "登入成功", user });
        }

    }
    public class RegisterRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}