CREATE DATABASE SIMS;
GO
USE SIMS;
GO

DROP TABLE IF EXISTS Attendance;
DROP TABLE IF EXISTS Grades;
DROP TABLE IF EXISTS CourseStudents;
DROP TABLE IF EXISTS Courses;
DROP TABLE IF EXISTS Users;
GO

-- 1. BẢNG USERS (CẬP NHẬT)
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Role VARCHAR(20) NOT NULL CHECK (Role IN ('Student', 'Teacher', 'Admin')),
    FullName NVARCHAR(100) NULL,
    Gender BIT NULL, -- 1 = Nam, 0 = Nữ
    PhoneNumber NVARCHAR(20) NULL,
    DateOfBirth DATE NULL,
    Department NVARCHAR(100) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1 -- 1 = Hoạt động, 0 = Đã vô hiệu hóa
);
GO

-- 2. BẢNG COURSES (MÔN HỌC)
CREATE TABLE Courses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Semester NVARCHAR(20) NOT NULL,
    TeacherId INT NULL, -- Cho phép NULL khi giáo viên bị xóa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Courses_Teacher FOREIGN KEY (TeacherId)
        REFERENCES Users(Id)
        ON DELETE SET NULL
);
GO

-- 3. BẢNG COURSESTUDENTS (ĐĂNG KÝ MÔN)
CREATE TABLE CourseStudents (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    StudentId INT NOT NULL,
    EnrolledAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CourseStudents_Course FOREIGN KEY (CourseId)
        REFERENCES Courses(Id)
        ON DELETE CASCADE,
    CONSTRAINT FK_CourseStudents_Student FOREIGN KEY (StudentId)
        REFERENCES Users(Id)
        ON DELETE NO ACTION,
    CONSTRAINT UQ_Course_Student UNIQUE (CourseId, StudentId)
);
GO

-- 4. BẢNG GRADES (ĐIỂM)
CREATE TABLE Grades (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    StudentId INT NOT NULL,
    Midterm FLOAT NULL,
    Final FLOAT NULL,
    Other FLOAT NULL,
    Total FLOAT NULL,
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Grades_Course FOREIGN KEY (CourseId)
        REFERENCES Courses(Id)
        ON DELETE CASCADE,
    CONSTRAINT FK_Grades_Student FOREIGN KEY (StudentId)
        REFERENCES Users(Id)
        ON DELETE NO ACTION,
    CONSTRAINT UQ_Grades_Course_Student UNIQUE (CourseId, StudentId)
);
GO

-- 5. BẢNG ATTENDANCE (ĐIỂM DANH)
CREATE TABLE Attendance (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    StudentId INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    IsPresent BIT NOT NULL,
    Note NVARCHAR(255) NULL,
    CONSTRAINT FK_Attendance_Course FOREIGN KEY (CourseId)
        REFERENCES Courses(Id)
        ON DELETE CASCADE,
    CONSTRAINT FK_Attendance_Student FOREIGN KEY (StudentId)
        REFERENCES Users(Id)
        ON DELETE NO ACTION,
    CONSTRAINT UQ_Attendance UNIQUE (CourseId, StudentId, AttendanceDate)
);
GO

-- Thêm 1 admin mẫu
INSERT INTO Users (Email, Role, FullName, Gender, PhoneNumber, DateOfBirth, Department)
VALUES ('khanhqewr1900@gmail.com', 'Admin', NULL, NULL, NULL, NULL, NULL);
GO

-- Cập nhật tất cả các bản ghi hiện có thành IsActive = 1
UPDATE Users
SET IsActive = 1
WHERE IsActive IS NULL;
GO
