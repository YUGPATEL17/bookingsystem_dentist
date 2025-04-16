using System;
using System.Collections.Generic;
using DentistAppointmentManagementSystem.Models;

namespace DentistAppointmentManagementSystem.DataStructures
{
    public class AppointmentBST
    {
        // The root node of the BST, which organizes appointments by patient name.
        private BSTNode? root;

        // Initializes an empty appointment BST.
        public AppointmentBST() => root = null;

        // Inserts an appointment into the BST using the patient's name as the key.
        public void Insert(Appointment appointment)
        {
            string key = appointment.PatientName;
            root = InsertRecursive(root, key, appointment);
        }

        // Recursively finds the correct location to insert the appointment.
        // If a node with the same patient name exists, adds the appointment to that node.
        private BSTNode InsertRecursive(BSTNode? node, string key, Appointment appointment)
        {
            if (node == null)
                return new BSTNode(key, appointment);
            int cmp = string.Compare(key, node.Key, StringComparison.OrdinalIgnoreCase);
            if (cmp == 0)
                node.Appointments.Add(appointment);
            else if (cmp < 0)
                node.Left = InsertRecursive(node.Left, key, appointment);
            else
                node.Right = InsertRecursive(node.Right, key, appointment);
            return node;
        }

        // Searches for a BST node based on the patient's name.
        // Returns the matching node if found, or null if not.
        public BSTNode? Search(string patientName) => SearchRecursive(root, patientName);

        // Recursively searches for the node with the given patient name.
        private BSTNode? SearchRecursive(BSTNode? node, string patientName)
        {
            if (node == null)
                return null;
            int cmp = string.Compare(patientName, node.Key, StringComparison.OrdinalIgnoreCase);
            if (cmp == 0)
                return node;
            else if (cmp < 0)
                return SearchRecursive(node.Left, patientName);
            else
                return SearchRecursive(node.Right, patientName);
        }

        // Performs an in-order traversal of the BST and returns all appointments in sorted order (by patient name).
        public List<Appointment> GetAppointmentsInOrder()
        {
            var list = new List<Appointment>();
            InOrderCollect(root, list);
            return list;
        }

        // Helper method to collect appointments via an in-order traversal.
        private void InOrderCollect(BSTNode? node, List<Appointment> list)
        {
            if (node == null)
                return;
            InOrderCollect(node.Left, list);
            list.AddRange(node.Appointments);
            InOrderCollect(node.Right, list);
        }

        // Removes an appointment by its ID from the BST.
        // Returns true if the appointment was removed.
        public bool RemoveByID(int appointmentID)
        {
            bool removed = false;
            root = RemoveRecursive(root, appointmentID, ref removed);
            return removed;
        }

        // Recursively searches for and removes the appointment.
        // If a node’s appointment list becomes empty, it adjusts the tree structure accordingly.
        private BSTNode? RemoveRecursive(BSTNode? node, int appointmentID, ref bool removed)
        {
            if (node == null)
                return null;
            node.Left = RemoveRecursive(node.Left, appointmentID, ref removed);
            node.Right = RemoveRecursive(node.Right, appointmentID, ref removed);
            Appointment? appToRemove = node.Appointments.Find(a => a.AppointmentID == appointmentID);
            if (appToRemove != null)
            {
                node.Appointments.Remove(appToRemove);
                removed = true;
            }
            // Remove the node if it has no appointments left.
            if (node.Appointments.Count == 0)
            {
                if (node.Left == null)
                    return node.Right;
                if (node.Right == null)
                    return node.Left;
                BSTNode minRight = FindMin(node.Right);
                node.Key = minRight.Key;
                node.Appointments = minRight.Appointments;
                node.Right = RemoveMin(node.Right);
            }
            return node;
        }

        // Finds the node with the minimum key in a subtree.
        private BSTNode FindMin(BSTNode node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        // Removes the node with the minimum key from a subtree.
        private BSTNode RemoveMin(BSTNode node)
        {
            if (node.Left == null)
                return node.Right!;
            node.Left = RemoveMin(node.Left);
            return node;
        }

        // Returns true if the BST contains no appointments.
        public bool IsEmpty() => root == null;
    }
}
