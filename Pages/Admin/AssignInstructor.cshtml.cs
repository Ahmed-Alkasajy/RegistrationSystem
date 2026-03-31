using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using student_online_system.Data;
using student_online_system.Models;
using InstructorModel = student_online_system.Models.Instructor;


namespace student_online_system.Pages.Admin

{
    public class AssignInstructorModel : PageModel
    {
        private readonly Db _db;

        public List<InstructorModel> Instructors { get; set; } = new();

        public List<Course> Courses { get; set; } = new();

        public string Message { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public AssignInstructorModel(Db db)
        {
            _db = db;
        }

        public void OnGet()
        {
            ProtectAdmin();
            LoadData();
        }

        public void OnPost()
        {
            ProtectAdmin();

            if (!int.TryParse(Request.Form["InstructorId"], out int instructorId) ||
                !int.TryParse(Request.Form["CourseId"], out int courseId))
            {
                ErrorMessage = "Invalid selection";
                LoadData();
                return;
            }

            try
            {
                using SqlConnection con = _db.GetConnection();
                using SqlCommand cmd = _db.CreateStoredProcCommand("AssignInstructorToCourse", con);

                cmd.Parameters.Add(new SqlParameter("@InstructorId", instructorId));
                cmd.Parameters.Add(new SqlParameter("@CourseId", courseId));

                con.Open();
                cmd.ExecuteNonQuery();

                Message = "Instructor assigned successfully";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadData();
        }

        private void LoadData()
        {
            Instructors = student_online_system.Models.Instructor.GetAll(_db);
            Courses = Course.GetAllSimple(_db);
        }

        private void ProtectAdmin()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Admin")
                Response.Redirect("/Login");
        }
    }
}
