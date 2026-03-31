using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages.Student
{
    public class MyCoursesModel : PageModel
    {
        private readonly Db _db;

        public List<Course> Courses { get; set; } = new();
        public string Message { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public MyCoursesModel(Db db)
        {
            _db = db;
        }

        public void OnGet()
        {
            ProtectStudent();
            LoadCourses();
        }

        public void OnPost()
        {
            ProtectStudent();

            if (!int.TryParse(Request.Form["CourseId"], out int courseId))
            {
                ErrorMessage = "Invalid course.";
                LoadCourses();
                return;
            }

            try
            {
                int studentId = GetStudentIdFromSessionUser();
                Course.DropStudentCourse(_db, studentId, courseId);
                Message = "Course dropped successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadCourses();
        }

        private void LoadCourses()
        {
            int studentId = GetStudentIdFromSessionUser();
            Courses = Course.GetStudentCourses(_db, studentId);
        }

        private int GetStudentIdFromSessionUser()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = new SqlCommand(
                "SELECT StudentId FROM Student WHERE UserId=@UserId",
                con
            );

            cmd.Parameters.AddWithValue("@UserId", userId);
            con.Open();

            object? result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        private void ProtectStudent()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Student")
                Response.Redirect("/Login");
        }
    }
}
