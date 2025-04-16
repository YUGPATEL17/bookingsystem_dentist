using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentistAppointmentManagementSystem.Models;
using DentistAppointmentManagementSystem.DataStructures;

namespace DentiDesk.test
{
    [TestClass]
    public class AppointmentBSTTests
    {
        // This test ensures that when we insert a single appointment for "Carlos Mendoza",
        // we can later retrieve that appointment using a search.
        [TestMethod]
        public void Insert_SingleAppointment_ShouldBeFoundBySearch()
        {
            var bst = new AppointmentBST();
            var appointment = new Appointment(1, "Carlos Mendoza", "Dr. María Gómez", DateTime.Today, "Routine Checkup");

            bst.Insert(appointment);
            var result = bst.Search("Carlos Mendoza");

            Assert.IsNotNull(result, "Expected to find a BST node for 'Carlos Mendoza'.");
            Assert.AreEqual(1, result.Appointments.Count, "There should be one appointment for 'Carlos Mendoza'.");
            Assert.AreEqual(appointment.AppointmentID, result.Appointments[0].AppointmentID, "The appointment IDs should match.");
        }

        // This test checks that if we insert two appointments for the same patient ("Carlos Mendoza"),
        // they are correctly grouped together and can be fetched as a single list.
        [TestMethod]
        public void Insert_MultipleAppointments_SamePatient_ShouldGroupAppointments()
        {
            var bst = new AppointmentBST();
            var appointment1 = new Appointment(1, "Carlos Mendoza", "Dr. Fatima Al-Mansouri", DateTime.Today, "General Checkup");
            var appointment2 = new Appointment(2, "Carlos Mendoza", "Dr. Hiroshi Nakamura", DateTime.Today.AddDays(1), "Follow-up Visit");

            bst.Insert(appointment1);
            bst.Insert(appointment2);
            var result = bst.Search("Carlos Mendoza");

            Assert.IsNotNull(result, "Search for 'Carlos Mendoza' should return a valid BST node.");
            Assert.AreEqual(2, result.Appointments.Count, "Both appointments for 'Carlos Mendoza' should be grouped together.");
        }

        // This test verifies that the in-order traversal of the BST returns appointments sorted alphabetically by patient name.
        // We insert appointments with names from different cultures and then check the sorted order.
        [TestMethod]
        public void GetAppointmentsInOrder_ShouldReturnSortedAppointments()
        {
            var bst = new AppointmentBST();
            var appointmentA = new Appointment(1, "Cheng Li", "Dr. Li", DateTime.Today, "Dental Checkup");
            var appointmentB = new Appointment(2, "Aisha Khan", "Dr. Khan", DateTime.Today.AddDays(1), "Regular Cleaning");
            var appointmentC = new Appointment(3, "Bao Nguyen", "Dr. Nguyen", DateTime.Today.AddDays(2), "Cavity Filling");

            bst.Insert(appointmentA);
            bst.Insert(appointmentB);
            bst.Insert(appointmentC);
            List<Appointment> sortedAppointments = bst.GetAppointmentsInOrder();

            Assert.AreEqual("Aisha Khan", sortedAppointments[0].PatientName, "First appointment should be for 'Aisha Khan'.");
            Assert.AreEqual("Bao Nguyen", sortedAppointments[1].PatientName, "Second appointment should be for 'Bao Nguyen'.");
            Assert.AreEqual("Cheng Li", sortedAppointments[2].PatientName, "Third appointment should be for 'Cheng Li'.");
        }

        // This test confirms that the RemoveByID function works correctly: removing an appointment with a known ID
        // should decrease the count for that patient, and trying to remove a non-existent ID should return false.
        [TestMethod]
        public void RemoveByID_ExistingAppointment_ShouldRemoveAppointment()
        {
            var bst = new AppointmentBST();
            var appointment1 = new Appointment(1, "Carlos Mendoza", "Dr. Fatima Al-Mansouri", DateTime.Today, "General Checkup");
            var appointment2 = new Appointment(2, "Carlos Mendoza", "Dr. Hiroshi Nakamura", DateTime.Today.AddDays(1), "Follow-up Visit");
            var appointment3 = new Appointment(3, "Akira Tanaka", "Dr. Maria Rossi", DateTime.Today.AddDays(2), "Consultation");

            bst.Insert(appointment1);
            bst.Insert(appointment2);
            bst.Insert(appointment3);

            bool removed = bst.RemoveByID(2);
            Assert.IsTrue(removed, "Removal should return true for an existing appointment ID.");

            var searchCarlos = bst.Search("Carlos Mendoza");
            Assert.IsNotNull(searchCarlos, "BST node for 'Carlos Mendoza' should still exist.");
            Assert.AreEqual(1, searchCarlos.Appointments.Count, "Only one appointment for 'Carlos Mendoza' should remain after removal.");

            bool removedNonExistent = bst.RemoveByID(999);
            Assert.IsFalse(removedNonExistent, "Removal should return false for a non-existent appointment ID.");
        }

        // This test checks that a newly created BST is empty,
        // and after inserting an appointment it is no longer empty.
        [TestMethod]
        public void IsEmpty_ShouldReturnTrueForNewBST_AndFalseAfterInsertion()
        {
            var bst = new AppointmentBST();

            Assert.IsTrue(bst.IsEmpty(), "A newly created BST should be empty.");

            bst.Insert(new Appointment(1, "Carlos Mendoza", "Dr. Fatima Al-Mansouri", DateTime.Today, "General Checkup"));

            Assert.IsFalse(bst.IsEmpty(), "BST should not be empty after an insertion.");
        }
    }
}
