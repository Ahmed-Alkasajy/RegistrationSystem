using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            Response.Redirect("/Login");
        }
    }
}
