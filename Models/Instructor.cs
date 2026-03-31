using System.Data;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FullName { get; set; } = "";

        public static List<Instructor> GetAll(Db db)
        {
            List<Instructor> list = new();

            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = new SqlCommand(
                "SELECT i.InstructorId, u.FullName " +
                "FROM Instructor i INNER JOIN UserAccount u ON u.UserId = i.UserId",
                con
            );

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Instructor
                {
                    InstructorId = (int)dr["InstructorId"],
                    FullName = dr["FullName"].ToString() ?? ""
                });
            }

            return list;
        }
    }
}
