using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly Db _db;

        public AuthController(Db db) => _db = db;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Please enter email and password" });

            var user = UserAccount.GetByEmailForLogin(_db, req.Email);
            if (user == null)
                return Unauthorized(new { error = "Invalid login" });

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(req.Email, user.PasswordHash, req.Password);

            if (result != PasswordVerificationResult.Success)
                return Unauthorized(new { error = "Invalid login" });

            HttpContext.Session.SetInt32("Login", 1);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", user.FullName);

            return Ok(new { role = user.Role, fullName = user.FullName });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok();
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            if (HttpContext.Session.GetInt32("Login") != 1)
                return Unauthorized();

            return Ok(new
            {
                userId = HttpContext.Session.GetInt32("UserId"),
                role = HttpContext.Session.GetString("Role"),
                fullName = HttpContext.Session.GetString("FullName")
            });
        }

        public record LoginRequest(string Email, string Password);
    }
}
