using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Pages.Instructor
{
    public class MaterialsModel : PageModel
    {
        private readonly Db _db;

        public List<MaterialView> Materials { get; set; } = new();
        public int CourseId { get; set; }
        public string ErrorMessage { get; set; } = "";

        public MaterialsModel(Db db)
        {
            _db = db;
        }

        public void OnGet(int courseId)
        {
            ProtectInstructor();

            CourseId = courseId;

            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd =
                _db.CreateStoredProcCommand("GetCourseMaterials", con);

            cmd.Parameters.AddWithValue("@CourseId", courseId);

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Materials.Add(new MaterialView
                {
                    FileName = dr["FileName"].ToString() ?? "",
                    FilePath = dr["FilePath"].ToString() ?? "",
                    UploadDate = (DateTime)dr["UploadDate"]
                });
            }
        }

        public void OnPost()
        {
            ProtectInstructor();

            int courseId = int.Parse(Request.Form["CourseId"]);
            CourseId = courseId;

            var file = Request.Form.Files["File"];
            if (file == null || file.Length == 0)
            {
                ErrorMessage = "Please select a file.";
                OnGet(courseId);
                return;
            }

            //save the file
            string uploadsFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            //save in the database
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd =
                _db.CreateStoredProcCommand("AddCourseMaterial", con);

            cmd.Parameters.AddWithValue("@CourseId", courseId);
            cmd.Parameters.AddWithValue("@UploadedByUserId", userId);
            cmd.Parameters.AddWithValue("@FileName", file.FileName);
            cmd.Parameters.AddWithValue("@ContentType", file.ContentType);
            cmd.Parameters.AddWithValue("@FilePath", "/uploads/" + file.FileName);

            con.Open();
            cmd.ExecuteNonQuery();

            Response.Redirect("/Instructor/Materials?courseId=" + courseId);
        }

        private void ProtectInstructor()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Instructor")
                Response.Redirect("/Login");
        }

        public class MaterialView
        {
            public string FileName { get; set; } = "";
            public string FilePath { get; set; } = "";
            public DateTime UploadDate { get; set; }
        }
    }
}
