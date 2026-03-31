using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Admin")
                Response.Redirect("/Login");
        }
    }
}
