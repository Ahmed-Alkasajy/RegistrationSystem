using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages.Instructor
{
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            ProtectInstructor();
        }

        private void ProtectInstructor()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Instructor")
                Response.Redirect("/Login");
        }
    }
}
