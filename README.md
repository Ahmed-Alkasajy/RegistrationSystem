# Online Registration System

A web-based **University Course Registration System** built with **ASP.NET Core Razor Pages** and **SQL Server**.
The system supports three main roles: **Admin**, **Student**, and **Instructor**, each with dedicated features for managing courses, enrollment, academic organization, and course materials.

---

## Overview

This project was developed as a university-style academic management system where:

* **Admins** manage departments, courses, users, and instructor assignments
* **Students** browse and enroll in courses
* **Instructors** manage assigned courses and upload course materials

The system is designed with a structured relational database, role-based access logic, and security-focused backend handling.

---

## Features

### Admin

* Create and manage **departments**
* Create and manage **courses**
* Create **students** and **instructors**
* Search users by **name** or **email**
* Assign instructors to courses
* Reset user passwords

### Student

* View available courses
* Enroll in courses
* Drop enrolled courses
* View registered courses

### Instructor

* View assigned courses
* View enrolled students in each course
* Upload course materials
* Access uploaded files

---

## Technologies Used

* **ASP.NET Core Razor Pages**
* **C#**
* **SQL Server**
* **ADO.NET**
* **Stored Procedures**
* **HTML / CSS / Bootstrap**

---

## Security Features

This project includes several backend security and data protection practices, including:

* **Password hashing**
* **Stored procedure-based database operations**
* **SQL Injection countermeasures**
* **Input validation**
* Controlled role-based access behavior

> Database interactions are designed to reduce direct unsafe query execution and improve security handling.

---

## Project Structure

```text
RegistrationSystem/
│
├── Data/
├── Models/
├── Pages/
├── Properties/
├── wwwroot/
├── .gitattributes
├── .gitignore
├── Program.cs
├── README.md
├── database.sql
├── appsettings.json
├── appsettings.Development.json
└── student online system.csproj
```

---

## Database Design

The system database includes the following main entities:

* **UserAccount**
* **Department**
* **Student**
* **Instructor**
* **Course**
* **CourseInstructor**
* **Enrollment**
* **CourseMaterial**

### Database Logic Includes

* Primary keys and foreign keys
* Unique constraints
* Capacity validation for courses
* Role-based user creation
* Instructor assignment restrictions
* Stored procedures for core system operations

---

## How to Run the Project

### 1) Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/YOUR_REPOSITORY_NAME.git
cd "YOUR_REPOSITORY_NAME"
```

---

### 2) Open the project

Open the project in:

* **Visual Studio 2022** (recommended)

Then open:

```text
student online system.csproj
```

or the solution file if available.

---

### 3) Create the database

Create a SQL Server database named:

```sql
regsystem
```

---

### 4) Run the database script

Open the SQL file:

```text
SqlScripts.txt
```

Then execute its contents in SQL Server Management Studio (SSMS) or your preferred SQL client.

This script will:

* Create all required tables
* Create stored procedures
* Apply constraints
* Insert sample departments and courses
* Insert a demo admin account

---

### 5) Configure the connection string

Open:

```json
appsettings.json
```

Update the connection string to match your SQL Server setup.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=regsystem;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Replace the server name if your SQL Server instance is different.

---

### 6) Run the project

Press:

```text
Ctrl + F5
```

or click **Run** in Visual Studio.

---

## Demo Admin Account

After running the SQL script, a demo admin account is created.

### Demo Login

* **Email:** `admin@example.com`
* **Password:** `admin123`

> You can modify this in the SQL script if needed.

---

## Business Rules Implemented

* A student **cannot enroll twice** in the same course
* A course **cannot exceed its capacity**
* A course can only have **one assigned instructor**
* A department **cannot be deleted** if it has assigned courses
* A course **cannot be deleted** if students are enrolled in it
* Duplicate emails, department names, and course codes are prevented

---

## Notes

* Uploaded files are stored and managed through the system
* The project follows a structured separation between:

  * **Data access**
  * **Models**
  * **Razor Pages**
  * **Static assets**

---

## Future Improvements

Possible future enhancements include:

* Authentication and authorization improvements
* Better dashboard design
* Search and filtering for courses
* Student grades / transcript features
* Notifications and announcements
* File upload validation improvements
* Pagination and reporting features

---

## Author

**Ahmed Yasser Alkasajy**
Software Engineering Student / Developer

---

## License

This project is for **educational, learning, and portfolio purposes**.
