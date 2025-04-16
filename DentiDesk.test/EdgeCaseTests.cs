using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentistAppointmentManagementSystem.DataStructures;
using DentistAppointmentManagementSystem.Models;

namespace DentiDesk.test
{
    [TestClass]
    public class EdgeCaseTests
    {
        // This test ensures that if we search for a patient in an empty BST,
        // no results are returned.
        [TestMethod]
        public void BST_Search_NonexistentPatient_ShouldReturnNull()
        {
            var bst = new AppointmentBST();
            var result = bst.Search("Nonexistent Patient");
            Assert.IsNull(result, "Searching an empty BST should return null.");
        }

        // This test confirms that trying to remove an appointment from an empty BST
        // correctly returns false, indicating that nothing was removed.
        [TestMethod]
        public void BST_Remove_FromEmptyBST_ShouldReturnFalse()
        {
            var bst = new AppointmentBST();
            bool removed = bst.RemoveByID(100);
            Assert.IsFalse(removed, "Removing from an empty BST should return false.");
        }
    }
}
