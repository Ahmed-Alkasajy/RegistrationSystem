using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Pages.Instructor
{
    public class CourseStudentsModel : PageModel
    {
        private readonly Db _db;

        public List<StudentView> Students { get; set; } = new();

        public CourseStudentsModel(Db db)
        {
            _db = db;
        }

        public void OnGet(int courseId)
        {
            ProtectInstructor();

            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = _db.CreateStoredProcCommand("GetCourseStudents", con);

            cmd.Parameters.AddWithValue("@CourseId", courseId);

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Students.Add(new StudentView
                {
                    FullName = dr["FullName"].ToString() ?? "",
                    EMail = dr["EMail"].ToString() ?? "",
                    EnrollDate = (DateTime)dr["EnrollDate"]
                });
            }
        }

        private void ProtectInstructor()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Instructor")
                Response.Redirect("/Login");
        }

        public class StudentView
        {
            public string FullName { get; set; } = "";
            public string EMail { get; set; } = "";
            public DateTime EnrollDate { get; set; }
        }
    }
}
