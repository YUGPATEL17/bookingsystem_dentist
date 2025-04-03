#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient; // Use Microsoft.Data.SqlClient instead of System.Data.SqlClient

namespace DentistAppointmentManagementSystem
{
    // Represents an appointment.
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public string PatientName { get; set; }
        public string DentistName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Description { get; set; }

        public Appointment(int id, string patient, string dentist, DateTime date, string description)
        {
            AppointmentID = id;
            PatientName = patient;
            DentistName = dentist;
            AppointmentDate = date;
            Description = description;
        }

        public override string ToString()
        {
            return $"ID: {AppointmentID}, Patient: {PatientName}, Dentist: {DentistName}, Date: {AppointmentDate}, Description: {Description}";
        }
    }

    // Data access class using ADO.NET to interact with the SQL database.
    public static class AppointmentDataAccess
    {
        // IMPORTANT: Update this connection string with your server and database details.
        private static string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=DentistAppointmentsDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

        // Retrieves all appointments from the database.
        public static List<Appointment> GetAllAppointments()
        {
            List<Appointment> appointments = new List<Appointment>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT AppointmentID, PatientName, DentistName, AppointmentDate, Description FROM Appointments";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string patient = reader.GetString(1);
                        string dentist = reader.GetString(2);
                        DateTime date = reader.GetDateTime(3);
                        string description = reader.GetString(4);
                        Appointment appointment = new Appointment(id, patient, dentist, date, description);
                        appointments.Add(appointment);
                    }
                }
            }
            return appointments;
        }

        // Inserts a new appointment into the database.
        public static void InsertAppointment(Appointment appointment)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Appointments (AppointmentID, PatientName, DentistName, AppointmentDate, Description) " +
                               "VALUES (@AppointmentID, @PatientName, @DentistName, @AppointmentDate, @Description)";
                using (SqlCommand command = new SqlCommand(query, connection))
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

        // Removes an appointment from the database.
        public static bool RemoveAppointment(int appointmentID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Appointments WHERE AppointmentID = @AppointmentID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AppointmentID", appointmentID);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Removes all appointments from the database.
        public static void RemoveAllAppointments()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Appointments";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    // BST node for the custom data structure.
    public class BSTNode
    {
        public string Key { get; set; } // Key is the patient's name.
        public List<Appointment> Appointments { get; set; }
        public BSTNode? Left { get; set; }
        public BSTNode? Right { get; set; }

        public BSTNode(string key, Appointment appointment)
        {
            Key = key;
            Appointments = new List<Appointment> { appointment };
            Left = null;
            Right = null;
        }
    }

    // Custom BST to store appointments keyed on patient name.
    public class AppointmentBST
    {
        private BSTNode? root;

        public AppointmentBST()
        {
            root = null;
        }

        // Inserts an appointment into the BST.
        public void Insert(Appointment appointment)
        {
            string key = appointment.PatientName;
            root = InsertRecursive(root, key, appointment);
        }

        private BSTNode InsertRecursive(BSTNode? node, string key, Appointment appointment)
        {
            if (node == null)
                return new BSTNode(key, appointment);

            int cmp = string.Compare(key, node.Key, StringComparison.OrdinalIgnoreCase);
            if (cmp == 0)
            {
                node.Appointments.Add(appointment);
            }
            else if (cmp < 0)
            {
                node.Left = InsertRecursive(node.Left, key, appointment);
            }
            else
            {
                node.Right = InsertRecursive(node.Right, key, appointment);
            }
            return node;
        }

        // Searches for appointments by patient name.
        public BSTNode? Search(string patientName)
        {
            return SearchRecursive(root, patientName);
        }

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

        // In-order traversal to display all appointments (legacy method).
        public void InOrderTraversal()
        {
            InOrderRecursive(root);
        }

        private void InOrderRecursive(BSTNode? node)
        {
            if (node == null)
                return;
            InOrderRecursive(node.Left);
            foreach (var app in node.Appointments)
                Console.WriteLine(app);
            InOrderRecursive(node.Right);
        }

        // Returns a list of appointments in order (in-order traversal).
        public List<Appointment> GetAppointmentsInOrder()
        {
            List<Appointment> list = new List<Appointment>();
            InOrderCollect(root, list);
            return list;
        }

        private void InOrderCollect(BSTNode? node, List<Appointment> list)
        {
            if (node == null)
                return;
            InOrderCollect(node.Left, list);
            list.AddRange(node.Appointments);
            InOrderCollect(node.Right, list);
        }

        // Removes an appointment by its ID from the BST.
        public bool RemoveByID(int appointmentID)
        {
            bool removed = false;
            root = RemoveRecursive(root, appointmentID, ref removed);
            return removed;
        }

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

            // Remove node if no appointments remain.
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

        private BSTNode FindMin(BSTNode node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        private BSTNode RemoveMin(BSTNode node)
        {
            if (node.Left == null)
                return node.Right!;
            node.Left = RemoveMin(node.Left);
            return node;
        }

        // Checks if the BST is empty.
        public bool IsEmpty()
        {
            return root == null;
        }
    }

    // Main Program class.
    class Program
    {
        // Global BST instance.
        static AppointmentBST appointmentBST = new AppointmentBST();
        // Next appointment ID is set after loading data from the database.
        static int nextAppointmentID = 1;

        static void Main(string[] args)
        {
            // Load existing appointments from the database.
            LoadAppointmentsFromDatabase();

            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                Console.Write("Enter your choice: ");
                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        AddAppointment();
                        break;
                    case "2":
                        RemoveAppointment();
                        break;
                    case "3":
                        SearchAppointment();
                        break;
                    case "4":
                        DisplayAppointmentsTable();
                        break;
                    case "5":
                        exit = true;
                        break;
                    case "6":
                        LoadAppointmentsFromFile();
                        break;
                    case "7":
                        RemoveAllAppointments();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option.");
                        break;
                }
            }
            Console.WriteLine("Exiting the application. Goodbye!");
        }

        // Displays the main menu in a table format.
        static void DisplayMenu()
        {
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("Dentist Appointment Management System".PadLeft(40));
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("+-------+-----------------------------------------+");
            Console.WriteLine("| Option| Description                             |");
            Console.WriteLine("+-------+-----------------------------------------+");
            Console.WriteLine("| 1     | Add Appointment                         |");
            Console.WriteLine("| 2     | Remove Appointment                      |");
            Console.WriteLine("| 3     | Search Appointment by Patient Name      |");
            Console.WriteLine("| 4     | Display All Appointments                |");
            Console.WriteLine("| 5     | Exit                                    |");
            Console.WriteLine("| 6     | Load Appointments from File             |");
            Console.WriteLine("| 7     | Remove All Appointments                 |");
            Console.WriteLine("+-------+-----------------------------------------+");
        }

        // Loads appointments from the SQL database and inserts them into the BST.
        static void LoadAppointmentsFromDatabase()
        {
            try
            {
                List<Appointment> appointments = AppointmentDataAccess.GetAllAppointments();
                foreach (var appointment in appointments)
                {
                    appointmentBST.Insert(appointment);
                    if (appointment.AppointmentID >= nextAppointmentID)
                        nextAppointmentID = appointment.AppointmentID + 1;
                }
                Console.WriteLine($"{appointments.Count} appointments loaded from the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading appointments from database: " + ex.Message);
            }
        }

        // Adds a new appointment to both the database and the BST.
        static void AddAppointment()
        {
            Console.Write("Enter patient name: ");
            string patientName = Console.ReadLine() ?? "";
            Console.Write("Enter dentist name: ");
            string dentistName = Console.ReadLine() ?? "";

            // Get the appointment date via step-by-step inputs.
            DateTime? appointmentDate = GetAppointmentDateFromUser();
            if (appointmentDate == null)
            {
                Console.WriteLine("Failed to get a valid appointment date.");
                return;
            }

            Console.Write("Enter description: ");
            string description = Console.ReadLine() ?? "";

            Appointment newAppointment = new Appointment(nextAppointmentID, patientName, dentistName, appointmentDate.Value, description);
            try
            {
                AppointmentDataAccess.InsertAppointment(newAppointment);
                appointmentBST.Insert(newAppointment);
                Console.WriteLine("Appointment added successfully with ID " + nextAppointmentID);
                nextAppointmentID++;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding appointment: " + ex.Message);
            }
        }

        // Helper method to get the appointment date from the user with separate inputs.
        static DateTime? GetAppointmentDateFromUser()
        {
            Console.Write("Enter appointment year (e.g. 2025): ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Invalid year input.");
                return null;
            }
            Console.Write("Enter appointment month (1-12): ");
            if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
            {
                Console.WriteLine("Invalid month input.");
                return null;
            }
            Console.Write("Enter appointment day (1-31): ");
            if (!int.TryParse(Console.ReadLine(), out int day) || day < 1 || day > 31)
            {
                Console.WriteLine("Invalid day input.");
                return null;
            }
            Console.Write("Enter appointment time (HH:mm): ");
            string? timeInput = Console.ReadLine();
            if (string.IsNullOrEmpty(timeInput))
            {
                Console.WriteLine("Invalid time input.");
                return null;
            }
            string[] timeParts = timeInput.Split(':');
            if (timeParts.Length != 2 ||
                !int.TryParse(timeParts[0], out int hour) ||
                !int.TryParse(timeParts[1], out int minute))
            {
                Console.WriteLine("Invalid time format.");
                return null;
            }
            try
            {
                DateTime appointmentDate = new DateTime(year, month, day, hour, minute, 0);
                return appointmentDate;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error constructing date: " + ex.Message);
                return null;
            }
        }

        // Removes an appointment (by ID) from both the database and the BST.
        static void RemoveAppointment()
        {
            Console.Write("Enter appointment ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            try
            {
                bool removedFromDB = AppointmentDataAccess.RemoveAppointment(id);
                bool removedFromBST = appointmentBST.RemoveByID(id);
                if (removedFromDB && removedFromBST)
                    Console.WriteLine("Appointment removed successfully.");
                else
                    Console.WriteLine("Appointment not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error removing appointment: " + ex.Message);
            }
        }

        // Searches for appointments by patient name using the BST.
        static void SearchAppointment()
        {
            Console.Write("Enter patient name to search: ");
            string patientName = Console.ReadLine() ?? "";
            BSTNode? resultNode = appointmentBST.Search(patientName);
            if (resultNode == null)
                Console.WriteLine("No appointments found for patient " + patientName);
            else
            {
                Console.WriteLine("Appointments for " + patientName + ":");
                foreach (var app in resultNode.Appointments)
                    Console.WriteLine(app);
            }
        }

        // Displays all appointments in a formatted table.
        static void DisplayAppointmentsTable()
        {
            List<Appointment> appointments = appointmentBST.GetAppointmentsInOrder();
            if (appointments.Count == 0)
            {
                Console.WriteLine("No appointments available.");
                return;
            }

            // Print table header
            Console.WriteLine(new string('-', 110));
            Console.WriteLine($"{"ID",5} {"Patient Name",20} {"Dentist Name",20} {"Date",20} {"Description",30}");
            Console.WriteLine(new string('-', 110));

            // Print each appointment in a formatted row.
            foreach (var app in appointments)
            {
                Console.WriteLine($"{app.AppointmentID,5} {app.PatientName,20} {app.DentistName,20} {app.AppointmentDate:yyyy-MM-dd HH:mm,20} {app.Description,30}");
            }
            Console.WriteLine(new string('-', 110));
        }

        // Loads appointments from a text file.
        static void LoadAppointmentsFromFile()
        {
            Console.Write("Enter the path to the text file: ");
            string? filePath = Console.ReadLine();

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("Invalid file path or file does not exist.");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    // Expected format: AppointmentID|PatientName|DentistName|AppointmentDate|Description
                    var parts = line.Split('|');
                    if (parts.Length != 5)
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue;
                    }

                    int id = int.Parse(parts[0]);
                    string patient = parts[1];
                    string dentist = parts[2];
                    DateTime date = DateTime.Parse(parts[3]);
                    string description = parts[4];

                    Appointment appointment = new Appointment(id, patient, dentist, date, description);

                    AppointmentDataAccess.InsertAppointment(appointment);
                    appointmentBST.Insert(appointment);

                    if (id >= nextAppointmentID)
                        nextAppointmentID = id + 1;
                }

                Console.WriteLine("Appointments successfully loaded from file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }
        }

        // Removes all appointments from both the database and the BST,
        // then resets the ID counter to 0.
        static void RemoveAllAppointments()
        {
            try
            {
                AppointmentDataAccess.RemoveAllAppointments();
                appointmentBST = new AppointmentBST(); // Reset the BST.
                nextAppointmentID = 0;
                Console.WriteLine("All appointments removed. Appointment counter reset to 0.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error removing all appointments: " + ex.Message);
            }
        }
    }
}
