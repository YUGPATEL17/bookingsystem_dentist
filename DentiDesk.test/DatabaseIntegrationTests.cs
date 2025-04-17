using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentistAppointmentManagementSystem.Models;
using DentistAppointmentManagementSystem.DataAccess;

namespace DentiDesk.test
{
    [TestClass]
    public class DatabaseIntegrationTests
    {
        /// <summary>
        /// This test checks that an appointment can be inserted into the database and then removed.
        /// It verifies that the appointment is present right after insertion and is completely removed afterwards.
        /// (Be sure to run these tests against a test database to prevent interference with production data.)
        /// </summary>
        
        [TestMethod]
        [TestCategory("Integration")]
        public void DatabaseIntegration_InsertAndRemoveAppointment()
        {
            // Create a test appointment with a unique ID to avoid conflict with existing records.
            int testAppointmentID = 9999;
            Appointment testAppointment = new Appointment(
                testAppointmentID,
                "Test Patient",
                "Test Dentist",
                DateTime.Now,
                "Test Description");

            try
            {
                // Insert the test appointment into the database.
                AppointmentDataAccess.InsertAppointment(testAppointment);

                // Retrieve all appointments and confirm the test appointment is present.
                List<Appointment> appointments = AppointmentDataAccess.GetAllAppointments();
                bool found = appointments.Exists(a => a.AppointmentID == testAppointmentID);
                Assert.IsTrue(found, "Test appointment should be found after insertion.");

                // Remove the test appointment.
                bool removed = AppointmentDataAccess.RemoveAppointment(testAppointmentID);
                Assert.IsTrue(removed, "Test appointment should be successfully removed.");

                // Retrieve appointments again to ensure the appointment is no longer present.
                appointments = AppointmentDataAccess.GetAllAppointments();
                bool stillFound = appointments.Exists(a => a.AppointmentID == testAppointmentID);
                Assert.IsFalse(stillFound, "Test appointment should no longer be in the database after removal.");
            }
            finally
            {
                // Cleanup: Ensure the test appointment is removed even if an error occurs.
                AppointmentDataAccess.RemoveAppointment(testAppointmentID);
            }
        }
    }
}
