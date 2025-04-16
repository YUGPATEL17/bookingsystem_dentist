using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DentistAppointmentManagementSystem.Models;

namespace DentistAppointmentManagementSystem.DataAccess
{
    public static class EmployeeDataAccess
    {
        private static string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=DentistAppointmentsDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

        public static List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT EmployeeId, Name, Designation, Password FROM Employees";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var emp = new Employee
                        {
                            EmployeeId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Designation = reader.GetString(2),
                            Password = reader.GetString(3)
                        };
                        employees.Add(emp);
                    }
                }
            }
            return employees;
        }

        public static void InsertEmployee(Employee emp)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Employees (EmployeeId, Name, Designation, Password) " +
                               "VALUES (@EmployeeId, @Name, @Designation, @Password)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    command.Parameters.AddWithValue("@Name", emp.Name);
                    command.Parameters.AddWithValue("@Designation", emp.Designation);
                    command.Parameters.AddWithValue("@Password", emp.Password);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool UpdateEmployeePassword(int employeeId, string newPassword)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Employees SET Password = @Password WHERE EmployeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", newPassword);
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

       
        public static bool RemoveEmployee(int employeeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Employees WHERE EmployeeId = @EmployeeId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
