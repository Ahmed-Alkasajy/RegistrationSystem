using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using student_online_system.Data;
using student_online_system.Models;

namespace student_online_system.Pages.Admin
{
    public class ResetPasswordModel : PageModel
    {
        private readonly Db _db;

        public string Search { get; set; } = "";
        public List<UserAccount> Users { get; set; } = new();

        public int SelectedUserId { get; set; } = 0;

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
        public string SearchMessage { get; set; } = "";

        public ResetPasswordModel(Db db)
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

            string action = Request.Form["Action"];
            Search = Request.Form["Search"];

            if (action == "Search")
            {
                DoSearch();
                return;
            }

            if (action == "Select")
            {
                if (int.TryParse(Request.Form["SelectedUserId"], out int id))
                    SelectedUserId = id;

                DoSearch(); 
                return;
            }

            if (action == "Reset")
            {
                if (!int.TryParse(Request.Form["SelectedUserId"], out int id))
                {
                    ErrorMessage = "invalid user selected";
                    DoSearch();
                    return;
                }

                SelectedUserId = id;

                string newPassword = Request.Form["NewPassword"];
                string confirm = Request.Form["ConfirmPassword"];

                if (newPassword != confirm)
                {
                    ErrorMessage = "Passwords do not match";
                    DoSearch();
                    return;
                }

                if (!IsStrongPassword(newPassword))
                {
                    ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter and one number";
                    DoSearch();
                    return;
                }

                try
                {
                    
                    PasswordHasher<string> hasher = new PasswordHasher<string>();
                    string hash = hasher.HashPassword(SelectedUserId.ToString(), newPassword);

                    UserAccount.AdminResetPassword(_db, SelectedUserId, hash);

                    SuccessMessage = "Password reset successfully";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }

                DoSearch();
                return;
            }

           
            DoSearch();
        }

        private void DoSearch()
        {
            if (string.IsNullOrWhiteSpace(Search))
            {
                Users = new();
                SearchMessage = "Enter a search term to find users.";
                return;
            }

            Users = UserAccount.SearchUsers(_db, Search.Trim());

            if (Users.Count == 0)
                SearchMessage = "No users found.";
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
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasUpper && hasDigit;
        }

        private void ProtectAdmin()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Admin")
                Response.Redirect("/Login");
        }
    }
}
