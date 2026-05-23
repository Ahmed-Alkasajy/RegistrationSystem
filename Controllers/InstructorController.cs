using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Controllers
{
    [ApiController]
    [Route("api/instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly Db _db;

        public InstructorController(Db db) => _db = db;

        private bool IsInstructor() =>
            HttpContext.Session.GetInt32("Login") == 1 &&
            HttpContext.Session.GetString("Role") == "Instructor";

        private int GetInstructorId()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = new("SELECT InstructorId FROM Instructor WHERE UserId=@UserId", con);
            cmd.Parameters.AddWithValue("@UserId", userId);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        [HttpGet("courses")]
        public IActionResult GetMyCourses()
        {
            if (!IsInstructor()) return Unauthorized();
            int instructorId = GetInstructorId();
            var list = new List<object>();
            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = _db.CreateStoredProcCommand("GetInstructorCourses", con);
            cmd.Parameters.AddWithValue("@InstructorId", instructorId);
            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new
                {
                    courseId = (int)dr["CourseId"],
                    courseCode = dr["CourseCode"].ToString(),
                    courseName = dr["CourseName"].ToString(),
                    departmentName = dr["DepartmentName"].ToString(),
                    capacity = (int)dr["Capacity"]
                });
            }
            return Ok(list);
        }

        [HttpGet("courses/{courseId}/students")]
        public IActionResult GetCourseStudents(int courseId)
        {
            if (!IsInstructor()) return Unauthorized();
            var list = new List<object>();
            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = _db.CreateStoredProcCommand("GetCourseStudents", con);
            cmd.Parameters.AddWithValue("@CourseId", courseId);
            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new
                {
                    fullName = dr["FullName"].ToString(),
                    email = dr["EMail"].ToString(),
                    enrollDate = ((DateTime)dr["EnrollDate"]).ToString("yyyy-MM-dd")
                });
            }
            return Ok(list);
        }

        [HttpGet("materials/{courseId}")]
        public IActionResult GetMaterials(int courseId)
        {
            if (!IsInstructor()) return Unauthorized();
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

        [HttpPost("materials")]
        public async Task<IActionResult> UploadMaterial([FromForm] int courseId, IFormFile file)
        {
            if (!IsInstructor()) return Unauthorized();
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Please select a file" });

            string uploadsFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);
            string filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = _db.CreateStoredProcCommand("AddCourseMaterial", con);
            cmd.Parameters.AddWithValue("@CourseId", courseId);
            cmd.Parameters.AddWithValue("@UploadedByUserId", userId);
            cmd.Parameters.AddWithValue("@FileName", file.FileName);
            cmd.Parameters.AddWithValue("@ContentType", file.ContentType);
            cmd.Parameters.AddWithValue("@FilePath", "/uploads/" + file.FileName);
            con.Open();
            cmd.ExecuteNonQuery();

            return Ok();
        }
    }
}
