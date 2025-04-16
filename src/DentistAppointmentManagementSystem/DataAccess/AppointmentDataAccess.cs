using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DentistAppointmentManagementSystem.Models;

namespace DentistAppointmentManagementSystem.DataAccess
{
    public static class AppointmentDataAccess
    {
        // Connection string for accessing the DentistAppointmentsDB database.
        private static string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=DentistAppointmentsDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

        // Retrieves all appointments from the database and returns them as a list of Appointment objects.
        public static List<Appointment> GetAllAppointments()
        {
            var appointments = new List<Appointment>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT AppointmentID, PatientName, DentistName, AppointmentDate, Description FROM Appointments";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string patient = reader.GetString(1);
                        string dentist = reader.GetString(2);
                        DateTime date = reader.GetDateTime(3);
                        string description = reader.GetString(4);
                        appointments.Add(new Appointment(id, patient, dentist, date, description));
                    }
                }
            }
            return appointments;
        }

        // Inserts the given appointment into the Appointments table of the database.
        public static void InsertAppointment(Appointment appointment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Appointments (AppointmentID, PatientName, DentistName, AppointmentDate, Description) " +
                               "VALUES (@AppointmentID, @PatientName, @DentistName, @AppointmentDate, @Description)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AppointmentID", appointment.AppointmentID);
                    command.Parameters.AddWithValue("@PatientName", appointment.PatientName);
                    command.Parameters.AddWithValue("@DentistName", appointment.DentistName);
                    command.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                    command.Parameters.AddWithValue("@Description", appointment.Description);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Removes the appointment with the specified ID from the database.
        // Returns true if the removal was successful (at least one row affected), or false otherwise.
        public static bool RemoveAppointment(int appointmentID)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Appointments WHERE AppointmentID = @AppointmentID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AppointmentID", appointmentID);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Deletes all appointment records from the Appointments table in the database.
        public static void RemoveAllAppointments()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Appointments";
                using (var command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
