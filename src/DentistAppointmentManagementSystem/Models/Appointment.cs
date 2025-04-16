#nullable enable
using System;

namespace DentistAppointmentManagementSystem.Models
{
    /// <summary>
    /// Represents a dental appointment with all essential details.
    /// </summary>
    public class Appointment
    {
        // Unique identifier for the appointment.
        public int AppointmentID { get; set; }

        // Full name of the patient.
        public string PatientName { get; set; }

        // Full name of the dentist.
        public string DentistName { get; set; }

        // Date and time when the appointment is scheduled.
        public DateTime AppointmentDate { get; set; }

        // Brief description or reason for the appointment.
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the Appointment class with the provided details.
        /// </summary>
        /// <param name="id">The unique appointment ID.</param>
        /// <param name="patient">The patient's full name.</param>
        /// <param name="dentist">The dentist's full name.</param>
        /// <param name="date">The scheduled date and time for the appointment.</param>
        /// <param name="description">A brief description of the appointment.</param>
        public Appointment(int id, string patient, string dentist, DateTime date, string description)
        {
            AppointmentID = id;
            PatientName = patient;
            DentistName = dentist;
            AppointmentDate = date;
            Description = description;
        }

        /// <summary>
        /// Returns a string representation of the appointment details.
        /// </summary>
        public override string ToString() =>
            $"ID: {AppointmentID}, Patient: {PatientName}, Dentist: {DentistName}, Date: {AppointmentDate}, Description: {Description}";
    }
}
