using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly Db _db;

        public AdminController(Db db) => _db = db;

        private bool IsAdmin() =>
            HttpContext.Session.GetInt32("Login") == 1 &&
            HttpContext.Session.GetString("Role") == "Admin";

        // ── Departments ──────────────────────────────────────────────

        [HttpGet("departments")]
        public IActionResult GetDepartments()
        {
            if (!IsAdmin()) return Unauthorized();
            return Ok(Department.GetAll(_db));
        }

        [HttpPost("departments")]
        public IActionResult CreateDepartment([FromBody] DepartmentRequest req)
        {
            if (!IsAdmin()) return Unauthorized();
            if (string.IsNullOrWhiteSpace(req.DepartmentName))
                return BadRequest(new { error = "Department name is required" });

            try
            {
                Department.Create(_db, req.DepartmentName.Trim());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("departments/{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            try
            {
                Department.Delete(_db, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── Courses ───────────────────────────────────────────────────

        [HttpGet("courses")]
        public IActionResult GetCourses()
        {
            if (!IsAdmin()) return Unauthorized();
            return Ok(Course.GetAll(_db));
        }

        [HttpGet("courses/simple")]
        public IActionResult GetCoursesSimple()
        {
            if (!IsAdmin()) return Unauthorized();
            return Ok(Course.GetAllSimple(_db));
        }

        [HttpPost("courses")]
        public IActionResult CreateCourse([FromBody] CourseRequest req)
        {
            if (!IsAdmin()) return Unauthorized();
            if (req.DepartmentId <= 0 || string.IsNullOrWhiteSpace(req.CourseCode) ||
                string.IsNullOrWhiteSpace(req.CourseName) || req.Capacity <= 0)
                return BadRequest(new { error = "Please enter valid course data" });

            try
            {
                Course.Create(_db, req.DepartmentId, req.CourseCode.Trim(), req.CourseName.Trim(), req.Capacity);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("courses/{id}")]
        public IActionResult DeleteCourse(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            try
            {
                Course.Delete(_db, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── Instructors ───────────────────────────────────────────────

        [HttpGet("instructors")]
        public IActionResult GetInstructors()
        {
            if (!IsAdmin()) return Unauthorized();
            return Ok(Instructor.GetAll(_db));
        }

        // ── Assign Instructor ─────────────────────────────────────────

        [HttpPost("assign-instructor")]
        public IActionResult AssignInstructor([FromBody] AssignRequest req)
        {
            if (!IsAdmin()) return Unauthorized();
            if (req.InstructorId <= 0 || req.CourseId <= 0)
                return BadRequest(new { error = "Invalid selection" });

            try
            {
                using SqlConnection con = _db.GetConnection();
                using SqlCommand cmd = _db.CreateStoredProcCommand("AssignInstructorToCourse", con);
                cmd.Parameters.AddWithValue("@InstructorId", req.InstructorId);
                cmd.Parameters.AddWithValue("@CourseId", req.CourseId);
                con.Open();
                cmd.ExecuteNonQuery();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── Users ─────────────────────────────────────────────────────

        [HttpPost("users")]
        public IActionResult CreateUser([FromBody] CreateUserRequest req)
        {
            if (!IsAdmin()) return Unauthorized();

            if (string.IsNullOrWhiteSpace(req.FullName) || string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Password) || string.IsNullOrWhiteSpace(req.Role))
                return BadRequest(new { error = "Please fill all fields" });

            if (!IsStrongPassword(req.Password))
                return BadRequest(new { error = "Password must be at least 8 characters and contain one uppercase letter and one number" });

            try
            {
                var hasher = new PasswordHasher<string>();
                string hash = hasher.HashPassword(req.Email, req.Password);
                UserAccount.Create(_db, req.FullName, req.Email, hash, req.Role);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("users/search")]
        public IActionResult SearchUsers([FromQuery] string q)
        {
            if (!IsAdmin()) return Unauthorized();
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { error = "Enter a search term" });

            var users = UserAccount.SearchUsers(_db, q.Trim());
            return Ok(users.Select(u => new { u.UserId, u.FullName, u.EMail, u.Role }));
        }

        [HttpPost("users/{id}/reset-password")]
        public IActionResult ResetPassword(int id, [FromBody] ResetPasswordRequest req)
        {
            if (!IsAdmin()) return Unauthorized();

            if (req.NewPassword != req.ConfirmPassword)
                return BadRequest(new { error = "Passwords do not match" });

            if (!IsStrongPassword(req.NewPassword))
                return BadRequest(new { error = "Password must be at least 8 characters and contain one uppercase letter and one number" });

            try
            {
                var hasher = new PasswordHasher<string>();
                string hash = hasher.HashPassword(id.ToString(), req.NewPassword);
                UserAccount.AdminResetPassword(_db, id, hash);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ── Helpers ───────────────────────────────────────────────────

        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
            bool hasUpper = false, hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsDigit(c)) hasDigit = true;
            }
            return hasUpper && hasDigit;
        }

        public record DepartmentRequest(string DepartmentName);
        public record CourseRequest(int DepartmentId, string CourseCode, string CourseName, int Capacity);
        public record AssignRequest(int InstructorId, int CourseId);
        public record CreateUserRequest(string FullName, string Email, string Password, string Role);
        public record ResetPasswordRequest(string NewPassword, string ConfirmPassword);
    }
}
