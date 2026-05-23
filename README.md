# Online Registration System

A web-based **University Course Registration System** built with **ASP.NET Core** and **SQL Server**.
The system supports three roles: **Admin**, **Student**, and **Instructor**, each with dedicated features for managing courses, enrollment, academic organization, and course materials.

The frontend uses Razor Pages as HTML shells, with all data loaded and submitted through a **REST API** via JavaScript `fetch()`.

---

## Overview

* **Admins** manage departments, courses, users, and instructor assignments
* **Students** browse and enroll in courses, and access course materials
* **Instructors** view assigned courses and upload course materials

---

## Features

### Admin
* Create and manage departments
* Create and manage courses
* Create student and instructor accounts
* Search users by name or email
* Assign instructors to courses
* Reset user passwords

### Student
* View available courses with seat counts
* Enroll in courses
* Drop enrolled courses
* View and download course materials

### Instructor
* View assigned courses and enrolled students
* Upload and manage course materials

---

## Technologies

* **ASP.NET Core 8.0** — Razor Pages (HTML shell) + Web API controllers
* **C#** — backend logic
* **SQL Server** — relational database
* **ADO.NET** — direct database access (no ORM)
* **Stored Procedures** — all core database operations
* **Bootstrap 5** — frontend styling
* **JavaScript (fetch API)** — all data requests and form submissions
* **DotNetEnv** — `.env` file support for local configuration

---

## Architecture

The system is split into two layers:

**API layer** (`/api/*`) — ASP.NET Core Web API controllers that handle all data operations and return JSON:

```
POST   /api/auth/login
POST   /api/auth/logout
GET    /api/admin/departments
POST   /api/admin/departments
DELETE /api/admin/departments/{id}
GET    /api/admin/courses
GET    /api/admin/courses/simple
POST   /api/admin/courses
DELETE /api/admin/courses/{id}
GET    /api/admin/instructors
POST   /api/admin/assign-instructor
POST   /api/admin/users
GET    /api/admin/users/search?q=
POST   /api/admin/users/{id}/reset-password
GET    /api/student/courses
DELETE /api/student/courses/{courseId}
GET    /api/student/available-courses
POST   /api/student/enroll
GET    /api/student/materials/{courseId}
GET    /api/instructor/courses
GET    /api/instructor/courses/{courseId}/students
GET    /api/instructor/materials/{courseId}
POST   /api/instructor/materials
```

**Page layer** (`/Admin/*`, `/Student/*`, `/Instructor/*`) — Razor Pages that serve HTML and use `fetch()` to call the API.

---

## Project Structure

```text
student online system/
├── Controllers/
│   ├── AuthController.cs
│   ├── AdminController.cs
│   ├── StudentController.cs
│   └── InstructorController.cs
├── Data/
│   └── Db.cs
├── Models/
│   ├── UserAccount.cs
│   ├── Student.cs
│   ├── Instructor.cs
│   ├── Course.cs
│   └── Department.cs
├── Pages/
│   ├── Login.cshtml
│   ├── Logout.cshtml
│   ├── Admin/
│   ├── Student/
│   └── Instructor/
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── lib/
│   └── uploads/
├── .env                  (not committed — copy from .env.example)
├── .env.example
├── .gitignore
├── Program.cs
├── appsettings.json
├── database.sql
└── student online system.csproj
```

---

## Database

**Database name:** `regsystem`

**Tables:** `UserAccount`, `Department`, `Student`, `Instructor`, `Course`, `CourseInstructor`, `Enrollment`, `CourseMaterial`

**Stored procedures:** `CreateUser`, `GetUserForLogin`, `GetDepartments`, `CreateDepartment`, `DeleteDepartment`, `GetCourses`, `CreateCourse`, `DeleteCourse`, `AssignInstructorToCourse`, `GetInstructorCourses`, `GetCourseStudents`, `EnrollStudent`, `GetStudentCourses`, `DropStudentCourse`, `GetCourseMaterials`, `AddCourseMaterial`, `SearchUsers`, `AdminResetPassword`

### Business Rules
* A student cannot enroll twice in the same course
* A course cannot exceed its capacity
* A course can only have one assigned instructor
* A department cannot be deleted if it has courses
* A course cannot be deleted if students are enrolled in it
* Duplicate emails, department names, and course codes are prevented

---

## How to Run

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/YOUR_REPO.git
cd "student online system"
```

### 2. Create the database

Create a SQL Server database named `regsystem`, then run `database.sql` in SSMS to create all tables, stored procedures, and seed data.

### 3. Configure the connection string

Copy `.env.example` to `.env` and set your SQL Server instance name:

```env
ConnectionStrings__ConnectionString=Data Source=YOUR_SERVER;Database=regsystem;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False
```

The connection string in `appsettings.json` is intentionally blank — `.env` overrides it at runtime.

### 4. Run the project

```bash
dotnet run
```

Or open `student online system.sln` in Visual Studio and press `Ctrl+F5`.

The app starts at `http://localhost:5010`.

### 5. Log in

A demo admin account is seeded by the SQL script:

* **Email:** `ahmed@gmail.com`
* **Password:** `123123`

---

## Security

* Passwords are hashed with `PasswordHasher<string>` (ASP.NET Core Identity)
* All database access uses parameterized stored procedures — no raw SQL with user input
* Role-based session checks on every API endpoint and page
* Connection string kept out of source control via `.env`

---

## Author

**Ahmed Yasser Alkasajy**

---

## License

This project is for educational, learning, and portfolio purposes.
