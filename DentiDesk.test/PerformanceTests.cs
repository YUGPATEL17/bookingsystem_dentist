using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentistAppointmentManagementSystem.DataStructures;
using DentistAppointmentManagementSystem.Models;

namespace DentiDesk.test
{
    [TestClass]
    public class PerformanceTests
    {
        // This test evaluates how fast the BST can handle a large number of insertions.
        // We create 10,000 appointment records with some duplicate names and measure the time taken to insert them.
        // The test asserts that the entire insertion process is completed in less than 1000 milliseconds.
        [TestMethod]
        public void BST_Insertion_PerformanceTest()
        {
            // Arrange: Initialize the BST and create a list of 10,000 appointments.
            var bst = new AppointmentBST();
            int numAppointments = 10000;
            var appointments = new List<Appointment>();

            // Generate appointments; patient names are cyclic to simulate duplicates.
            for (int i = 0; i < numAppointments; i++)
            {
                string patientName = "Patient" + (i % 100);
                appointments.Add(new Appointment(i, patientName, "Dr. Performance", DateTime.Today.AddMinutes(i), "Performance Test"));
            }

            // Start the stopwatch to measure performance.
            Stopwatch sw = Stopwatch.StartNew();

            // Act: Insert each appointment into the BST.
            foreach (var app in appointments)
            {
                bst.Insert(app);
            }
            sw.Stop();

            // Assert: Verify that the entire insertion process completes in under 1000 milliseconds.
            long elapsedMs = sw.ElapsedMilliseconds;
            Assert.IsTrue(elapsedMs < 1000, $"Insertion of {numAppointments} appointments took too long: {elapsedMs}ms");
        }
    }
}
