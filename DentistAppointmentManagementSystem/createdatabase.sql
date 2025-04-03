-- Create the DentistAppointmentsDB database if it does not exist.
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DentistAppointmentsDB')
BEGIN
    CREATE DATABASE DentistAppointmentsDB;
END;
GO

-- Switch to the newly created database.
USE DentistAppointmentsDB;
GO

-- Create the Appointments table for the Dentist Appointment Management System.
IF OBJECT_ID('Appointments', 'U') IS NOT NULL
    DROP TABLE Appointments;
GO

CREATE TABLE Appointments (
    AppointmentID INT PRIMARY KEY,
    PatientName VARCHAR(100) NOT NULL,
    DentistName VARCHAR(100) NOT NULL,
    AppointmentDate DATETIME NOT NULL,
    Description VARCHAR(255)
);
GO
