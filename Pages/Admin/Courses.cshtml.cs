using Microsoft.AspNetCore.Mvc.RazorPages;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages.Admin
{
    public class CoursesModel : PageModel
    {
        private readonly Db _db;

        public List<Department> Departments { get; set; } = new();
        public List<Course> Courses { get; set; } = new();

        public string ErrorMessage { get; set; } = "";
        public string Message { get; set; } = "";

        public CoursesModel(Db db)
        {
            _db = db;
        }

        public void OnGet()
        {
            ProtectAdmin();
            LoadAll();
        }

        public void OnPost()
        {
            ProtectAdmin();

            string courseid = Request.Form["DeleteCourseId"];

            if (!string.IsNullOrWhiteSpace(courseid))
            {
                int courseId = int.Parse(courseid);

                try
                {
                    Course.Delete(_db, courseId);
                    Message = "Course deleted successfully.";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

                LoadAll();
                return;
            }

            string depIdStr = Request.Form["DepartmentId"];
            string code = Request.Form["CourseCode"];
            string name = Request.Form["CourseName"];
            string capStr = Request.Form["Capacity"];

            
            if (!int.TryParse(depIdStr, out int departmentId) ||
                string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrWhiteSpace(name) ||
                !int.TryParse(capStr, out int capacity) ||
                capacity <= 0)
            {
                ErrorMessage = "Please enter valid course data.";
                LoadAll();
                return;
            }

            try
            {
                Course.Create(_db, departmentId, code.Trim(), name.Trim(), capacity);
                Message = "Course created successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadAll();
        }

        private void LoadAll()
        {
            Departments = Department.GetAll(_db);
            Courses = Course.GetAll(_db);
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
