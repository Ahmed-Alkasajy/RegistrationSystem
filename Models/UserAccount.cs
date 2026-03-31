using System.Data;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Models
{
    public class UserAccount
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string EMail { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "";
        public static UserAccount? GetByEmailForLogin(Db db, string email)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("GetUserForLogin", con);

            cmd.Parameters.Add(new SqlParameter("@EMail", SqlDbType.VarChar) { Value = email });

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())
                return null;

            return new UserAccount
            {
                UserId = (int)dr["UserId"],
                FullName = dr["FullName"].ToString() ?? "",
                EMail = dr["EMail"].ToString() ?? "",
                PasswordHash = dr["PasswordHash"].ToString() ?? "",
                Role = dr["Role"].ToString() ?? ""
            };
        }

            public static void Create(Db db, string fullName, string email, string passwordHash, string role)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("CreateUser", con);

            cmd.Parameters.Add("@FullName", SqlDbType.VarChar, 100).Value = fullName;
            cmd.Parameters.Add("@EMail", SqlDbType.VarChar, 120).Value = email;
            cmd.Parameters.Add("@PasswordHash", SqlDbType.VarChar, 400).Value = passwordHash;
            cmd.Parameters.Add("@Role", SqlDbType.VarChar, 20).Value = role;

            con.Open();
            cmd.ExecuteNonQuery();
        }
        public static List<UserAccount> SearchUsers(Db db, string search)
        {
            List<UserAccount> list = new();

            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("SearchUsers", con);

            cmd.Parameters.Add(new SqlParameter("@Search", SqlDbType.VarChar) { Value = search });

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new UserAccount
                {
                    UserId = (int)dr["UserId"],
                    FullName = dr["FullName"].ToString() ?? "",
                    EMail = dr["EMail"].ToString() ?? "",
                    Role = dr["Role"].ToString() ?? ""
                });
            }

            return list;
        }

        public static void AdminResetPassword(Db db, int userId, string passwordHash)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("AdminResetPassword", con);

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
            cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarChar) { Value = passwordHash });

            con.Open();
            cmd.ExecuteNonQuery();
        }



    }
}

