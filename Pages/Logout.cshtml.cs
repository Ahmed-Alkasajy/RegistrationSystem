using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
            HttpContext.Session.Clear();
            Response.Redirect("/Login");
        }

        public void OnPost()
        {
            HttpContext.Session.Clear();
            Response.Redirect("/Login");
        }
    }
}
