using System.Collections.Generic;
using DentistAppointmentManagementSystem.Models;

namespace DentistAppointmentManagementSystem.DataStructures
{
    /// <summary>
    /// Represents a node in a binary search tree used for storing appointments.
    /// Each node is keyed by the patient's name and may contain multiple appointments with the same name.
    /// </summary>
    public class BSTNode
    {
        // The key is based on the patient's name.
        public string Key { get; set; }

        // A list of appointments associated with the patient.
        public List<Appointment> Appointments { get; set; }

        // Reference to the left child node.
        public BSTNode? Left { get; set; }

        // Reference to the right child node.
        public BSTNode? Right { get; set; }

        /// <summary>
        /// Initializes a new instance of the BSTNode class with a given key and an initial appointment.
        /// </summary>
        /// <param name="key">The patient's name used as the key.</param>
        /// <param name="appointment">The appointment to be added to the node.</param>
        public BSTNode(string key, Appointment appointment)
        {
            Key = key;
            Appointments = new List<Appointment> { appointment };
            Left = null;
            Right = null;
        }
    }
}
