-- Create the database if it does not exist.
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DentistAppointmentsDB')
BEGIN
    CREATE DATABASE DentistAppointmentsDB;
END
GO

USE DentistAppointmentsDB;
GO

-- Create the Appointments table.
IF OBJECT_ID('dbo.Appointments', 'U') IS NOT NULL
    DROP TABLE dbo.Appointments;
GO
CREATE TABLE dbo.Appointments
(
    AppointmentID INT PRIMARY KEY,
    PatientName NVARCHAR(100) NOT NULL,
    DentistName NVARCHAR(100) NOT NULL,
    AppointmentDate DATETIME NOT NULL,
    Description NVARCHAR(255) NULL
);
GO

-- Create the Employees table.
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL
    DROP TABLE dbo.Employees;
GO
CREATE TABLE dbo.Employees
(
    EmployeeId INT PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Designation NVARCHAR(100) NOT NULL,
    Password NVARCHAR(50) NOT NULL
);
GO
