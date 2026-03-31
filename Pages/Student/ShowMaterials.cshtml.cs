using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Pages.Student
{
    public class MaterialsModel : PageModel
    {
        private readonly Db _db;

        public List<MaterialView> Materials { get; set; } = new();

        public MaterialsModel(Db db)
        {
            _db = db;
        }

        public void OnGet(int courseId)
        {
            ProtectStudent();

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

        private void ProtectStudent()
        {
            int login = HttpContext.Session.GetInt32("Login") ?? 0;
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (login != 1 || role != "Student")
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
