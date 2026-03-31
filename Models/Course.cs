using System.Data;
using Microsoft.Data.SqlClient;
using student_online_system.Data;

namespace student_online_system.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = "";
        public string CourseName { get; set; } = "";
        public int Capacity { get; set; }
        public string DepartmentName { get; set; } = "";
        public int EnrolledCount { get; set; }
        public int SeatsLeft => Capacity - EnrolledCount;


        public static List<Course> GetAll(Db db)
        {
            List<Course> list = new List<Course>();

            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("GetCourses", con);

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Course
                {
                    CourseId = (int)dr["CourseId"],
                    CourseCode = dr["CourseCode"].ToString() ?? "",
                    CourseName = dr["CourseName"].ToString() ?? "",
                    Capacity = (int)dr["Capacity"],
                    DepartmentName = dr["DepartmentName"].ToString() ?? ""
                });
            }

            return list;
        }

        public static void Create(Db db, int departmentId, string code, string name, int capacity)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("CreateCourse", con);

            cmd.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int) { Value = departmentId });
            cmd.Parameters.Add(new SqlParameter("@CourseCode", SqlDbType.VarChar) { Value = code });
            cmd.Parameters.Add(new SqlParameter("@CourseName", SqlDbType.VarChar) { Value = name });
            cmd.Parameters.Add(new SqlParameter("@Capacity", SqlDbType.Int) { Value = capacity });

            con.Open();
            cmd.ExecuteNonQuery();
        }
    
    public static List<Course> GetAllSimple(Db db)
        {
            List<Course> list = new List<Course>();

            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = new SqlCommand(
                "SELECT CourseId, CourseCode, CourseName FROM Course ORDER BY CourseCode",
                con
            );

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Course
                {
                    CourseId = (int)dr["CourseId"],
                    CourseCode = dr["CourseCode"].ToString() ?? "",
                    CourseName = dr["CourseName"].ToString() ?? ""
                });
            }

            return list;
        }
        public static List<Course> GetWithSeats(Db db)
        {
            List<Course> list = new();

            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = new SqlCommand(
                @"SELECT c.CourseId, c.CourseCode, c.CourseName, c.Capacity,
                 (SELECT COUNT(*) FROM Enrollment e WHERE e.CourseId = c.CourseId) AS EnrolledCount
          FROM Course c
          ORDER BY c.CourseCode",
                con
            );

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Course
                {
                    CourseId = (int)dr["CourseId"],
                    CourseCode = dr["CourseCode"].ToString() ?? "",
                    CourseName = dr["CourseName"].ToString() ?? "",
                    Capacity = (int)dr["Capacity"],
                    EnrolledCount = (int)dr["EnrolledCount"]
                });
            }

            return list;
        }

        public static List<Course> GetStudentCourses(Db db, int studentId)
        {
            List<Course> list = new();

            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("GetStudentCourses", con);

            cmd.Parameters.Add(new SqlParameter("@StudentId", SqlDbType.Int) { Value = studentId });

            con.Open();
            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Course
                {
                    CourseId = (int)dr["CourseId"],
                    CourseCode = dr["CourseCode"].ToString() ?? "",
                    CourseName = dr["CourseName"].ToString() ?? "",
                    DepartmentName = dr["DepartmentName"].ToString() ?? ""
                });
            }

            return list;
        }
        public static void DropStudentCourse(Db db, int studentId, int courseId)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("DropStudentCourse", con);

            cmd.Parameters.Add(new SqlParameter("@StudentId", SqlDbType.Int) { Value = studentId });
            cmd.Parameters.Add(new SqlParameter("@CourseId", SqlDbType.Int) { Value = courseId });

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public static void Delete(Db db, int courseId)
        {
            using SqlConnection con = db.GetConnection();
            using SqlCommand cmd = db.CreateStoredProcCommand("DeleteCourse", con);

            cmd.Parameters.Add(new SqlParameter("@CourseId", SqlDbType.Int) { Value = courseId });

            con.Open();
            cmd.ExecuteNonQuery();
        }



    }
}

