using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Controllers
{
    [ApiController]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private readonly Db _db;

        public StudentController(Db db) => _db = db;

        private bool IsStudent() =>
            HttpContext.Session.GetInt32("Login") == 1 &&
            HttpContext.Session.GetString("Role") == "Student";

        private int GetStudentId()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = new("SELECT StudentId FROM Student WHERE UserId=@UserId", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        [HttpGet("courses")]
        public IActionResult GetMyCourses()
        {
            if (!IsStudent()) return Unauthorized();
            int studentId = GetStudentId();
            var courses = Course.GetStudentCourses(_db, studentId);
            return Ok(courses.Select(c => new
            {
                c.CourseId, c.CourseCode, c.CourseName, c.DepartmentName
            }));
        }

        [HttpDelete("courses/{courseId}")]
        public IActionResult DropCourse(int courseId)
        {
            if (!IsStudent()) return Unauthorized();
            try
            {
                int studentId = GetStudentId();
                Course.DropStudentCourse(_db, studentId, courseId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("available-courses")]
        public IActionResult GetAvailableCourses()
        {
            if (!IsStudent()) return Unauthorized();
            var courses = Course.GetWithSeats(_db);
            return Ok(courses.Select(c => new
            {
                c.CourseId, c.CourseCode, c.CourseName,
                c.Capacity, c.EnrolledCount, c.SeatsLeft
            }));
        }

        [HttpPost("enroll")]
        public IActionResult Enroll([FromBody] EnrollRequest req)
        {
            if (!IsStudent()) return Unauthorized();
            try
            {
                int studentId = GetStudentId();
                using SqlConnection con = _db.GetConnection();
                using SqlCommand cmd = _db.CreateStoredProcCommand("EnrollStudent", con);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
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

        [HttpGet("materials/{courseId}")]
        public IActionResult GetMaterials(int courseId)
        {
            if (!IsStudent()) return Unauthorized();
            var list = new List<object>();
            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = _db.CreateStoredProcCommand("GetCourseMaterials", con);
            cmd.Parameters.AddWithValue("@CourseId", courseId);
            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new
                {
                    fileName = dr["FileName"].ToString(),
                    filePath = dr["FilePath"].ToString(),
                    uploadDate = ((DateTime)dr["UploadDate"]).ToString("yyyy-MM-dd")
                });
            }
            return Ok(list);
        }

        public record EnrollRequest(int CourseId);
    }
}
