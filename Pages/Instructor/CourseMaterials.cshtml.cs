using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages.Instructor
{
    public class CourseMaterialsModel : PageModel
    {
        private readonly Db _db;

        public List<Course> Courses { get; set; } = new();

        public CourseMaterialsModel(Db db)
        {
            _db = db;
        }

        public void OnGet()
        {
            ProtectInstructor();

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            using SqlConnection con = _db.GetConnection();

            using SqlCommand cmd = new SqlCommand("SELECT InstructorId FROM Instructor WHERE UserId = @UserId",con);

            cmd.Parameters.AddWithValue("@UserId", userId);

            con.Open();
            int instructorId = (int)cmd.ExecuteScalar(); //returns only the first column of the first row and it returns an object so casting is needed

            using SqlCommand courseCmd =_db.CreateStoredProcCommand("GetInstructorCourses", con);

            courseCmd.Parameters.AddWithValue("@InstructorId", instructorId);

            using SqlDataReader dr = courseCmd.ExecuteReader();

            while (dr.Read())
            {
                Courses.Add(new Course
                {
                    CourseId = (int)dr["CourseId"],
                    CourseCode = dr["CourseCode"].ToString() ?? "",
                    CourseName = dr["CourseName"].ToString() ?? "",
                    DepartmentName = dr["DepartmentName"].ToString() ?? "",
                    Capacity = (int)dr["Capacity"]
                });
            }
        }

        private void ProtectInstructor()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                Response.Redirect("/Login");
                return;
            }

            if (login != 1 || role != "Instructor")
                Response.Redirect("/Login");
        }
    }
}
