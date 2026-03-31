using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages.Student
{
    public class EnrollModel : PageModel
    {
        private readonly Db _db;

        public List<Course> Courses { get; set; } = new();
        public string Message { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public EnrollModel(Db db)
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

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            try
            {
                
                using SqlConnection con = _db.GetConnection();
                using SqlCommand cmd = new SqlCommand(
                    "SELECT StudentId FROM Student WHERE UserId=@UserId",
                    con
                );

                cmd.Parameters.AddWithValue("@UserId", userId);
                con.Open();
                int studentId = (int)cmd.ExecuteScalar();

                
                using SqlCommand enrollCmd = _db.CreateStoredProcCommand("EnrollStudent", con);
                enrollCmd.Parameters.AddWithValue("@StudentId", studentId);
                enrollCmd.Parameters.AddWithValue("@CourseId", courseId);
                enrollCmd.ExecuteNonQuery();

                Message = "Enrolled successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadCourses();
        }

        private void LoadCourses()
        {
            Courses = Course.GetWithSeats(_db);
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
