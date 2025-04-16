#nullable enable

namespace DentistAppointmentManagementSystem.Models
{
    /// <summary>
    /// Represents an employee in the dental office system.
    /// </summary>
    public class Employee
    {
        // Unique 5-digit identifier for the employee.
        public int EmployeeId { get; set; }

        // The full name of the employee.
        public string Name { get; set; } = string.Empty;

        // The employee's job title or designation.
        public string Designation { get; set; } = string.Empty;

        // The employee's password (formatted as a 5-digit string).
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Returns a string representation of the employee, including ID, name, designation, and password.
        /// </summary>
        public override string ToString() =>
            $"ID: {EmployeeId}, Name: {Name}, Designation: {Designation}, Password: {Password}";
    }
}
