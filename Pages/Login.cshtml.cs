using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace student_online_system.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login == 1 && role == "Admin") return RedirectToPage("/Admin/Dashboard");
            if (login == 1 && role == "Student") return RedirectToPage("/Student/Dashboard");
            if (login == 1 && role == "Instructor") return RedirectToPage("/Instructor/Dashboard");

            return Page();
        }
    }
}
