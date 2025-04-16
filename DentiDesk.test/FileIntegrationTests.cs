using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentistAppointmentManagementSystem.DataAccess;
using DentistAppointmentManagementSystem.DataStructures;
using DentistAppointmentManagementSystem.Models;

namespace DentiDesk.test
{
    [TestClass]
    public class FileIntegrationTests
    {
        // This test verifies that a file with two correctly formatted appointment records 
        // is processed as expected. It checks that both appointments are loaded into the BST 
        // and that the appointment counter (nextAppointmentID) is updated correctly.
        [TestMethod]
        public void FileIntegration_ValidData_ShouldLoadAppointments()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                var validLines = new[]
                {
                    "101|Alice|Dr. Smith|2025-01-01 10:00|Routine Checkup",
                    "102|Bob|Dr. Adams|2025-01-02 11:00|Follow-up"
                };
                File.WriteAllLines(tempFile, validLines);

                var bst = new AppointmentBST();
                int nextAppointmentID = 1;

                // Process the file by splitting each line and adding appointments to the BST.
                string[] lines = File.ReadAllLines(tempFile);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length != 5)
                        continue;

                    int id = int.Parse(parts[0]);
                    string patient = parts[1];
                    string dentist = parts[2];
                    DateTime date = DateTime.Parse(parts[3]);
                    string description = parts[4];
                    var appointment = new Appointment(id, patient, dentist, date, description);
                    bst.Insert(appointment);
                    if (id >= nextAppointmentID)
                        nextAppointmentID = id + 1;
                }

                // Confirm that two appointments were loaded and the counter is updated as expected.
                List<Appointment> loadedAppointments = bst.GetAppointmentsInOrder();
                Assert.AreEqual(2, loadedAppointments.Count, "Expected two appointments loaded from a valid file.");
                Assert.AreEqual(103, nextAppointmentID, "Next Appointment ID should be updated to 103 after valid file load.");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        // This test ensures that if a file contains both one valid and one malformed line, 
        // only the valid appointment is processed. The test confirms that the BST contains 
        // only the valid record and that the appointment counter is updated correctly.
        [TestMethod]
        public void FileIntegration_MalformedData_ShouldSkipInvalidLines()
        {
            string tempFile = Path.GetTempFileName();
            try
            {
                var lines = new[]
                {
                    "201|Charlie|Dr. Jones|2025-02-15 09:30|Consultation", // Valid line.
                    "NotAValidLineWithoutProperDelimiter"                // Malformed line.
                };
                File.WriteAllLines(tempFile, lines);

                var bst = new AppointmentBST();
                int nextAppointmentID = 1;

                // Process the file and skip any lines that don't have exactly 5 parts.
                string[] fileLines = File.ReadAllLines(tempFile);
                foreach (var line in fileLines)
                {
                    var parts = line.Split('|');
                    if (parts.Length != 5)
                        continue;

                    int id = int.Parse(parts[0]);
                    string patient = parts[1];
                    string dentist = parts[2];
                    DateTime date = DateTime.Parse(parts[3]);
                    string description = parts[4];
                    var appointment = new Appointment(id, patient, dentist, date, description);
                    bst.Insert(appointment);
                    if (id >= nextAppointmentID)
                        nextAppointmentID = id + 1;
                }

                // Confirm that only the valid appointment was loaded.
                List<Appointment> loadedAppointments = bst.GetAppointmentsInOrder();
                Assert.AreEqual(1, loadedAppointments.Count, "Expected one appointment loaded, as the malformed line should be skipped.");
                Assert.AreEqual("Charlie", loadedAppointments[0].PatientName, "The valid appointment should be for 'Charlie'.");
                Assert.AreEqual(202, nextAppointmentID, "Next Appointment ID should be updated to 202 after processing valid data.");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
