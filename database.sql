// create a data base named : regsystem
// after executing these scrtipts in the database , an admin has been craeted:
//admin username: ahmed@gmail.com
//admin password: 123123

=========================================
Entity Database Scripts
=========================================

-- USERS (Admin / Student / Instructor)
CREATE TABLE dbo.[UserAccount] (
    UserId        INT IDENTITY(1,1) PRIMARY KEY,
    FullName      VARCHAR(100) NOT NULL,
    EMail         VARCHAR(120) NOT NULL UNIQUE,
    PasswordHash  VARCHAR(400) NOT NULL,     -- Store hashed password (PasswordHasher)
    [Role]        VARCHAR(20)  NOT NULL      -- 'Admin' | 'Student' | 'Instructor'
);

CREATE TABLE dbo.Department (
    DepartmentId   INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL UNIQUE
);


CREATE TABLE dbo.Student (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    UserId    INT NOT NULL UNIQUE,
    DepartmentId INT NULL,
    CONSTRAINT FK_Student_User FOREIGN KEY (UserId) REFERENCES dbo.UserAccount(UserId),
    CONSTRAINT FK_Student_Dep FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(DepartmentId)
);

CREATE TABLE dbo.Instructor (
    InstructorId INT IDENTITY(1,1) PRIMARY KEY,
    UserId       INT NOT NULL UNIQUE,
    DepartmentId INT NULL,
    CONSTRAINT FK_Instructor_User FOREIGN KEY (UserId) REFERENCES dbo.UserAccount(UserId),
    CONSTRAINT FK_Instructor_Dep FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(DepartmentId)
);

CREATE TABLE dbo.Course (
    CourseId      INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentId  INT NOT NULL,
    CourseCode    VARCHAR(20) NOT NULL UNIQUE,
    CourseName    VARCHAR(120) NOT NULL,
    Capacity      INT NOT NULL,
    CONSTRAINT FK_Course_Dep FOREIGN KEY (DepartmentId) REFERENCES dbo.Department(DepartmentId),
    CONSTRAINT CK_Course_Capacity CHECK (Capacity > 0)
);

CREATE TABLE dbo.CourseInstructor (
    CourseInstructorId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId           INT NOT NULL,
    InstructorId       INT NOT NULL,
    CONSTRAINT UQ_Course_Instructor UNIQUE (CourseId, InstructorId),
    CONSTRAINT FK_CI_Course FOREIGN KEY (CourseId) REFERENCES dbo.Course(CourseId),
    CONSTRAINT FK_CI_Instructor FOREIGN KEY (InstructorId) REFERENCES dbo.Instructor(InstructorId)
);

CREATE TABLE dbo.Enrollment (
    EnrollmentId  INT IDENTITY(1,1) PRIMARY KEY,
    StudentId     INT NOT NULL,
    CourseId      INT NOT NULL,
    EnrollDate    DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Enrollment UNIQUE (StudentId, CourseId),
    CONSTRAINT FK_Enroll_Student FOREIGN KEY (StudentId) REFERENCES dbo.Student(StudentId),
    CONSTRAINT FK_Enroll_Course FOREIGN KEY (CourseId) REFERENCES dbo.Course(CourseId)
);

CREATE TABLE dbo.CourseMaterial (
    MaterialId    INT IDENTITY(1,1) PRIMARY KEY,
    CourseId      INT NOT NULL,
    UploadedByUserId INT NOT NULL,
    FileName      VARCHAR(260) NOT NULL,
    ContentType   VARCHAR(100) NOT NULL,   -- e.g. application/pdf, image/png
    FilePath      VARCHAR(500) NOT NULL,   -- stored path under wwwroot/uploads/...
    UploadDate    DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Material_Course FOREIGN KEY (CourseId) REFERENCES dbo.Course(CourseId),
    CONSTRAINT FK_Material_User FOREIGN KEY (UploadedByUserId) REFERENCES dbo.UserAccount(UserId)
);

GO

=========================================
Stored Procedures
=========================================

CREATE OR ALTER PROCEDURE dbo.DropStudentCourse
    @StudentId INT,
    @CourseId INT
AS
BEGIN
    DELETE FROM dbo.Enrollment
    WHERE StudentId = @StudentId AND CourseId = @CourseId;
END
GO


CREATE OR ALTER PROCEDURE dbo.CreateUser
    @FullName VARCHAR(100),
    @EMail VARCHAR(120),
    @PasswordHash VARCHAR(400),
    @Role VARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.UserAccount(FullName, EMail, PasswordHash, [Role])
    VALUES(@FullName, @EMail, @PasswordHash, @Role);

    DECLARE @NewUserId INT = SCOPE_IDENTITY();

    -- Auto create student/instructor row if needed
    IF (@Role = 'Student')
        INSERT INTO dbo.Student(UserId) VALUES(@NewUserId);

    IF (@Role = 'Instructor')
        INSERT INTO dbo.Instructor(UserId) VALUES(@NewUserId);
END
GO

-- Get password hash + role by email (used by Login)
CREATE OR ALTER PROCEDURE dbo.GetUserForLogin
    @EMail VARCHAR(120)
AS
BEGIN
    SELECT UserId, EMail, PasswordHash, [Role], FullName
    FROM dbo.UserAccount
    WHERE EMail = @EMail;
END
GO

-- Departments
CREATE OR ALTER PROCEDURE dbo.CreateDepartment
    @DepartmentName VARCHAR(100)
AS
BEGIN
    INSERT INTO dbo.Department(DepartmentName) VALUES (@DepartmentName);
END
GO

CREATE OR ALTER PROCEDURE dbo.GetDepartments
AS
BEGIN
    SELECT DepartmentId, DepartmentName FROM dbo.Department ORDER BY DepartmentName;
END
GO

-- Courses
CREATE OR ALTER PROCEDURE dbo.CreateCourse
    @DepartmentId INT,
    @CourseCode VARCHAR(20),
    @CourseName VARCHAR(120),
    @Capacity INT
AS
BEGIN
    INSERT INTO dbo.Course(DepartmentId, CourseCode, CourseName, Capacity)
    VALUES (@DepartmentId, @CourseCode, @CourseName, @Capacity);
END
GO

CREATE OR ALTER PROCEDURE dbo.GetCourses
AS
BEGIN
    SELECT c.CourseId, c.CourseCode, c.CourseName, c.Capacity,
           d.DepartmentName
    FROM dbo.Course c
    INNER JOIN dbo.Department d ON d.DepartmentId = c.DepartmentId
    ORDER BY c.CourseCode;
END
GO

-- Seats used (count enrollments)
CREATE OR ALTER PROCEDURE dbo.GetCourseSeats
    @CourseId INT
AS
BEGIN
    SELECT
      (SELECT COUNT(*) FROM dbo.Enrollment e WHERE e.CourseId = @CourseId) AS EnrolledCount,
      (SELECT Capacity FROM dbo.Course c WHERE c.CourseId = @CourseId) AS Capacity;
END
GO

-- Enroll student (checks capacity)
CREATE OR ALTER PROCEDURE dbo.EnrollStudent
    @StudentId INT,
    @CourseId INT
AS
BEGIN
    DECLARE @EnrolledCount INT = (SELECT COUNT(*) FROM dbo.Enrollment WHERE CourseId=@CourseId);
    DECLARE @Capacity INT = (SELECT Capacity FROM dbo.Course WHERE CourseId=@CourseId);

    IF (@EnrolledCount >= @Capacity)
    BEGIN
        RAISERROR('Course is full', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Enrollment(StudentId, CourseId)
    VALUES(@StudentId, @CourseId);
END
GO

-- Student courses
CREATE OR ALTER PROCEDURE dbo.GetStudentCourses
    @StudentId INT
AS
BEGIN
    SELECT c.CourseId, c.CourseCode, c.CourseName, d.DepartmentName, e.EnrollDate
    FROM dbo.Enrollment e
    INNER JOIN dbo.Course c ON c.CourseId = e.CourseId
    INNER JOIN dbo.Department d ON d.DepartmentId = c.DepartmentId
    WHERE e.StudentId = @StudentId
    ORDER BY c.CourseCode;
END
GO

-- Instructor courses
CREATE OR ALTER PROCEDURE dbo.GetInstructorCourses
    @InstructorId INT
AS
BEGIN
    SELECT c.CourseId, c.CourseCode, c.CourseName, d.DepartmentName, c.Capacity
    FROM dbo.CourseInstructor ci
    INNER JOIN dbo.Course c ON c.CourseId = ci.CourseId
    INNER JOIN dbo.Department d ON d.DepartmentId = c.DepartmentId
    WHERE ci.InstructorId = @InstructorId
    ORDER BY c.CourseCode;
END
GO

-- Students in a course (for instructor)
CREATE OR ALTER PROCEDURE dbo.GetCourseStudents
    @CourseId INT
AS
BEGIN
    SELECT s.StudentId, u.FullName, u.EMail, e.EnrollDate
    FROM dbo.Enrollment e
    INNER JOIN dbo.Student s ON s.StudentId = e.StudentId
    INNER JOIN dbo.UserAccount u ON u.UserId = s.UserId
    WHERE e.CourseId = @CourseId
    ORDER BY u.FullName;
END
GO

-- Assign instructor to course (admin)
CREATE OR ALTER PROCEDURE dbo.AssignInstructorToCourse
    @InstructorId INT,
    @CourseId INT
AS
BEGIN
    INSERT INTO dbo.CourseInstructor(InstructorId, CourseId)
    VALUES(@InstructorId, @CourseId);
END
GO

-- Course material
CREATE OR ALTER PROCEDURE dbo.AddCourseMaterial
    @CourseId INT,
    @UploadedByUserId INT,
    @FileName VARCHAR(260),
    @ContentType VARCHAR(100),
    @FilePath VARCHAR(500)
AS
BEGIN
    INSERT INTO dbo.CourseMaterial(CourseId, UploadedByUserId, FileName, ContentType, FilePath)
    VALUES(@CourseId, @UploadedByUserId, @FileName, @ContentType, @FilePath);
END
GO

CREATE OR ALTER PROCEDURE dbo.GetCourseMaterials
    @CourseId INT
AS
BEGIN
    SELECT MaterialId, FileName, ContentType, FilePath, UploadDate
    FROM dbo.CourseMaterial
    WHERE CourseId = @CourseId
    ORDER BY UploadDate DESC;
END
GO


CREATE OR ALTER PROCEDURE dbo.GetDepartments
AS
BEGIN
    SELECT DepartmentId, DepartmentName
    FROM dbo.Department
    ORDER BY DepartmentId ASC;
END
GO

CREATE OR ALTER PROCEDURE dbo.SearchUsers
    @Search VARCHAR(120)
AS
BEGIN
    SELECT TOP 50
        UserId,
        FullName,
        EMail,
        [Role]
    FROM dbo.UserAccount
    WHERE
        EMail LIKE '%' + @Search + '%'
        OR FullName LIKE '%' + @Search + '%'
    ORDER BY EMail;
END
GO


CREATE OR ALTER PROCEDURE dbo.AdminResetPassword
    @UserId INT,
    @PasswordHash VARCHAR(400)
AS
BEGIN
    UPDATE dbo.UserAccount
    SET PasswordHash = @PasswordHash
    WHERE UserId = @UserId;
END
GO


CREATE OR ALTER PROCEDURE dbo.DeleteDepartment
    @DepartmentId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Course WHERE DepartmentId = @DepartmentId)
    BEGIN
        RAISERROR('cannot delete department it has assigned courses', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.Department
    WHERE DepartmentId = @DepartmentId;
END
GO

CREATE OR ALTER PROCEDURE dbo.DeleteCourse
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Enrollment WHERE CourseId = @CourseId)
    BEGIN
        RAISERROR('cannot delete course it has enrolled students', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.Course
    WHERE CourseId = @CourseId;
END
GO

CREATE OR ALTER PROCEDURE dbo.AssignInstructorToCourse
    @InstructorId INT,
    @CourseId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.CourseInstructor WHERE CourseId = @CourseId)
    BEGIN
        RAISERROR('this course already has an assigned instructor', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.CourseInstructor(InstructorId, CourseId)
    VALUES(@InstructorId, @CourseId);
END
GO

CREATE OR ALTER PROCEDURE dbo.CreateUser
    @FullName VARCHAR(100),
    @EMail VARCHAR(120),
    @PasswordHash VARCHAR(400),
    @Role VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.UserAccount WHERE EMail = @EMail)
    BEGIN
        RAISERROR('there is already a user with this email', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.UserAccount(FullName, EMail, PasswordHash, [Role])
    VALUES(@FullName, @EMail, @PasswordHash, @Role);

    DECLARE @NewUserId INT = SCOPE_IDENTITY();

    IF (@Role = 'Student')
        INSERT INTO dbo.Student(UserId) VALUES(@NewUserId);

    IF (@Role = 'Instructor')
        INSERT INTO dbo.Instructor(UserId) VALUES(@NewUserId);
END
GO

CREATE OR ALTER PROCEDURE dbo.CreateDepartment
    @DepartmentName VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Department WHERE DepartmentName = @DepartmentName)
    BEGIN
        RAISERROR('there is already a department with this name', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Department(DepartmentName)
    VALUES (@DepartmentName);
END
GO

CREATE OR ALTER PROCEDURE dbo.CreateCourse
    @DepartmentId INT,
    @CourseCode VARCHAR(20),
    @CourseName VARCHAR(120),
    @Capacity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Course
        WHERE CourseCode = @CourseCode
    )
    BEGIN
        RAISERROR('there is already a course with this code id', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Course(DepartmentId, CourseCode, CourseName, Capacity)
    VALUES (@DepartmentId, @CourseCode, @CourseName, @Capacity);
END
GO

ALTER TABLE dbo.CourseInstructor
ADD CONSTRAINT UQ_CourseInstructor_Course UNIQUE (CourseId);
GO


=========================================
Sample Data (Adimn + Departments + Courses)
=========================================

INSERT INTO dbo.UserAccount (FullName,EMail,PasswordHash,Role) VALUES ('ahmed','ahmed@gmail.com','AQAAAAIAAYagAAAAEHyqSSl3XGVkOjyi6UNx7UDh8dU4yxiiJG5T47A36EeL6xZxzZE3eEOm0eNMk/EeIQ==','Admin');
GO
INSERT INTO dbo.Department(DepartmentName) VALUES ('Software Engineering'), ('Computer Science');
INSERT INTO dbo.Course(DepartmentId, CourseCode, CourseName, Capacity)
VALUES
(1,'SE201','OOP',40),
(1,'SE305','Server Side Programming',40),
(2,'CS210','Database Systems',35);
