using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages.Student
{
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            ProtectStudent();
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
