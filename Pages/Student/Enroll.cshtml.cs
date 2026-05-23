using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages.Student
{
    public class EnrollModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("Login") != 1 || HttpContext.Session.GetString("Role") != "Student")
                return RedirectToPage("/Login");
            return Page();
        }
    }
}
