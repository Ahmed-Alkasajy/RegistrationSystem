using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages.Admin
{
    public class CoursesModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("Login") != 1 || HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToPage("/Login");
            return Page();
        }
    }
}
