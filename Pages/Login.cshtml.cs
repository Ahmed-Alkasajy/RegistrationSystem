using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Db _db;

        public string ErrorMessage { get; set; } = "";
        public string PrefilledEmail { get; set; } = "";
        public bool RememberMeChecked { get; set; } = false;

        public LoginModel(Db db)
        {
            _db = db;
        }

        public IActionResult OnGet()
        {

            
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login == 1 && role == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            else if (login == 1 && role == "Student")
            {
                return RedirectToPage("/Student/Dashboard");
            }
            else if (login == 1 && role == "Instructor")
            {
                return RedirectToPage("/Instructor/Dashboard");
            }

            return Page();

        }

        public void OnPost()
        {

            string email = Request.Form["EMail"];
            string password = Request.Form["Password"];
            string rememberMe = Request.Form["RememberMe"];

            PrefilledEmail = email;
            //RememberMeChecked = (rememberMe == "1");

            
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Please enter email and password";
                return;
            }

            var user = UserAccount.GetByEmailForLogin(_db, email);
            if (user == null)
            {
                ErrorMessage = "Invalid login";
                return;
            }

            
            PasswordHasher<string> hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(email, user.PasswordHash, password);

            if (result != PasswordVerificationResult.Success)
            {
                ErrorMessage = "Invalid login";
                return;
            }

            //session
            HttpContext.Session.SetInt32("Login", 1);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", user.FullName);

           

            //role
            if (user.Role == "Admin") Response.Redirect("/Admin/Dashboard");
            else if (user.Role == "Instructor") Response.Redirect("/Instructor/Dashboard");
            else Response.Redirect("/Student/Dashboard");
        }
    }
}
