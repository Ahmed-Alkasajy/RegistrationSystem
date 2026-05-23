using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages.Instructor
{
    public class CourseMaterialsModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("Login") != 1 || HttpContext.Session.GetString("Role") != "Instructor")
                return RedirectToPage("/Login");
            return Page();
        }
    }
}
