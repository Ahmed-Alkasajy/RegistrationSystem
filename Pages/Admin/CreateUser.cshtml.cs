using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages.Admin
{
    public class CreateUserModel : PageModel
    {
        private readonly Db _db;

        public string Message { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public CreateUserModel(Db db)
        {
            _db = db;
        }

        public void OnGet()
        {
            ProtectAdmin();
        }

        public void OnPost()
        {
            ProtectAdmin();

            string fullName = Request.Form["FullName"];
            string email = Request.Form["EMail"];
            string password = Request.Form["Password"];
            string role = Request.Form["Role"];

            // basic server-side validation (simple)
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                ErrorMessage = "Please fill all fields";
                return;
            }

            if (!IsStrongPassword(password))
            {
                ErrorMessage =
                    "Password must be at least 8 characters long and contain at least one uppercase letter and one number";
                return;
            }

            try
            {
                // Hash password
                PasswordHasher<string> hasher = new PasswordHasher<string>();
                string hash = hasher.HashPassword(email, password);

                
                UserAccount.Create(_db, fullName, email, hash, role);

                Message = $"User created successfully ({role}).";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void ProtectAdmin()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Admin")
                Response.Redirect("/Login");
        }
        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpper = true;

                if (char.IsDigit(c))
                    hasDigit = true;
            }

            return hasUpper && hasDigit;
        }
    }
}
