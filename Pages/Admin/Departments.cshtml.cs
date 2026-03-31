using Microsoft.AspNetCore.Mvc.RazorPages;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages.Admin
{
    public class DepartmentsModel : PageModel
    {
        private readonly Db _db;

        public List<Department> Departments { get; set; } = new();
        public string ErrorMessage { get; set; } = "";

        public DepartmentsModel(Db db)
        {
            _db = db;
        }

        public void OnGet()
        {
            ProtectAdmin();
            LoadDepartments();
        }

        public void OnPost()
        {
            ProtectAdmin();

            string depid = Request.Form["DeleteDepartmentId"];
            if (!string.IsNullOrEmpty(depid))
            {
                int departmentId = int.Parse(depid);

                try
                {
                    Department.Delete(_db, departmentId);
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

                LoadDepartments();
                return;
            }

            string name = Request.Form["DepartmentName"];

            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorMessage = "Department name is required";
                LoadDepartments();
                return;
            }

            try
            {
                Department.Create(_db, name);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadDepartments();
        }

        private void LoadDepartments()
        {
            Departments = Department.GetAll(_db);
        }

        public void DeleteDepartment(int departmentId)
        {
            ProtectAdmin();

            try
            {
                Department.Delete(_db, departmentId);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            LoadDepartments();
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
