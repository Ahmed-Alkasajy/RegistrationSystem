using System.Data;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = "";

        
        public static List<Department> GetAll(Db db)
        {
            List<Department> list = new List<Department>();

            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("GetDepartments", con);

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Department
                {
                    DepartmentId = (int)dr["DepartmentId"],
                    DepartmentName = dr["DepartmentName"].ToString() ?? ""
                });
            }

            return list;
        }

       
        public static void Create(Db db, string name)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("CreateDepartment", con);

            cmd.Parameters.Add(new SqlParameter("@DepartmentName", SqlDbType.VarChar)
            {
                Value = name
            });

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public static void Delete(Db db, int departmentId)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("DeleteDepartment", con);

            cmd.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int)
            {
                Value = departmentId
            });

            con.Open();
            cmd.ExecuteNonQuery();
        }

    }
}
