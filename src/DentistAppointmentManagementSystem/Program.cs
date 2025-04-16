#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using Spectre.Console;

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

    // Data access class using ADO.NET.
    public static class AppointmentDataAccess
    {
        // IMPORTANT: Update this connection string with your server and database details.
        private static string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=DentistAppointmentsDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

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
                        appointments.Add(new Appointment(id, patient, dentist, date, description));
                    }
                }
            }
            return appointments;
        }

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
        public string Key { get; set; } // Patient name is used as the key.
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

        public AppointmentBST() => root = null;

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
                node.Appointments.Add(appointment);
            else if (cmp < 0)
                node.Left = InsertRecursive(node.Left, key, appointment);
            else
                node.Right = InsertRecursive(node.Right, key, appointment);

            return node;
        }

        public BSTNode? Search(string patientName) => SearchRecursive(root, patientName);

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

        public bool IsEmpty() => root == null;
    }

    // Main Program class.
    class Program
    {
        static AppointmentBST appointmentBST = new AppointmentBST();
        static int nextAppointmentID = 1;

        static string masterPassword = "admin123";
        static string employeePassword = "emp123";

        // Helper methods that allow cancellation.
        static string? AskStringOrBack(string prompt)
        {
            var result = AnsiConsole.Ask<string>($"{prompt} (or type 'back' to cancel):");
            if (result.Trim().ToLower() == "back")
                return null;
            return result;
        }

        static int? AskIntOrBack(string prompt)
        {
            var input = AnsiConsole.Ask<string>($"{prompt} (or type 'back' to cancel):");
            if (input.Trim().ToLower() == "back")
                return null;
            if (int.TryParse(input, out int value))
                return value;
            AnsiConsole.MarkupLine("[red]Invalid number.[/]");
            return AskIntOrBack(prompt);
        }

        static void Main(string[] args)
        {
            int accessLevel = LoginPage();
            if (accessLevel == 1)
                AnsiConsole.MarkupLine("[green]Admin access granted.[/]");
            else if (accessLevel == 2)
                AnsiConsole.MarkupLine("[green]Employee access granted.[/]");

            AnsiConsole.Clear();
            LoadAppointmentsFromDatabase();

            bool exit = false;
            while (!exit)
            {
                AnsiConsole.Clear();

                string headerArt = @"
██████  ███████ ███    ██ ████████ ██ ██████  ███████ ███████ ██   ██ 
██   ██ ██      ████   ██    ██    ██ ██   ██ ██      ██      ██  ██  
██   ██ █████   ██ ██  ██    ██    ██ ██   ██ █████   ███████ █████   
██   ██ ██      ██  ██ ██    ██    ██ ██   ██ ██           ██ ██  ██  
██████  ███████ ██   ████    ██    ██ ██████  ███████ ███████ ██   ██ 
                                                                     ";
                AnsiConsole.MarkupLine(headerArt);
                AnsiConsole.MarkupLine("[bold underline]Dentist Appointment Management System[/]");
                AnsiConsole.MarkupLine("");

                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold cyan]Select an option:[/]")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "Add Appointment",
                            "Remove Appointment",
                            "Search Appointment by Patient Name",
                            "Display All Appointments",
                            "Load Appointments from File",
                            "Remove All Appointments",
                            "Exit"
                        }));

                AnsiConsole.MarkupLine("");

                // Each operation returns true if it was completed (and thus should pause),
                // or false if the user cancelled (via "back") and no pause is needed.
                bool completed = choice switch
                {
                    "Add Appointment" => AddAppointment(),
                    "Remove Appointment" => RemoveAppointment(),
                    "Search Appointment by Patient Name" => SearchAppointment(),
                    "Display All Appointments" => DisplayAppointmentsTable(),
                    "Load Appointments from File" => LoadAppointmentsFromFile(),
                    "Remove All Appointments" => RemoveAllAppointments(),
                    "Exit" => (exit = true, true).Item2,
                    _ => false,
                };

                if (!exit && completed)
                    Pause();
            }
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[green]Exiting the application. Goodbye![/]");
        }

        // Pause is only called if the operation actually completed.
        static void Pause()
        {
            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        static int LoginPage()
        {
            while (true)
            {
                string option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold cyan]Welcome to DentiDesk[/]\n[bold]Choose an option:[/]")
                        .AddChoices(new[] { "Login", "Reset/Change Password", "Exit" }));

                if (option == "Exit")
                {
                    Environment.Exit(0);
                }
                else if (option == "Reset/Change Password")
                {
                    ResetPasswords();
                    continue;
                }
                else if (option == "Login")
                {
                    string input = AnsiConsole.Prompt(
                        new TextPrompt<string>("Enter password:")
                            .Secret());
                    if (input == masterPassword)
                        return 1;
                    else if (input == employeePassword)
                        return 2;
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Incorrect password. Try again.[/]");
                        continue;
                    }
                }
            }
        }

        static void ResetPasswords()
        {
            AnsiConsole.MarkupLine("[bold yellow]Reset/Change Passwords[/]");
            string? newMaster = AskStringOrBack("Enter new master (admin) password:");
            if (newMaster == null) return;
            string? newEmployee = AskStringOrBack("Enter new employee password:");
            if (newEmployee == null) return;
            masterPassword = newMaster;
            employeePassword = newEmployee;
            AnsiConsole.MarkupLine("[green]Passwords updated successfully.[/]");
            Pause();
            AnsiConsole.Clear();
        }

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
                AnsiConsole.MarkupLine($"[yellow]{appointments.Count} appointments loaded from the database.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Error loading appointments from database: [/]" + ex.Message);
            }
        }

        static bool AddAppointment()
        {
            string? patientName = AskStringOrBack("Enter patient name");
            if (patientName == null) return false;

            string? dentistName = AskStringOrBack("Enter dentist name");
            if (dentistName == null) return false;

            DateTime? appointmentDate = GetAppointmentDateFromUser();
            if (appointmentDate == null)
            {
                AnsiConsole.MarkupLine("[red]Appointment creation canceled or failed.[/]");
                return false;
            }

            string? description = AskStringOrBack("Enter description");
            if (description == null) return false;

            Appointment newAppointment = new Appointment(nextAppointmentID, patientName, dentistName, appointmentDate.Value, description);
            try
            {
                AppointmentDataAccess.InsertAppointment(newAppointment);
                appointmentBST.Insert(newAppointment);
                AnsiConsole.MarkupLine($"[green]Appointment added successfully with ID {nextAppointmentID}.[/]");
                nextAppointmentID++;
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Error adding appointment: [/]" + ex.Message);
                return false;
            }
        }

        static DateTime? GetAppointmentDateFromUser()
        {
            int? year = AskIntOrBack("Enter appointment year (e.g. 2025)");
            if (year == null) return null;

            int? month = AskIntOrBack("Enter appointment month (1-12)");
            if (month == null) return null;
            if (month < 1 || month > 12)
            {
                AnsiConsole.MarkupLine("[red]Invalid month provided.[/]");
                return null;
            }

            int? day = AskIntOrBack("Enter appointment day (1-31)");
            if (day == null) return null;
            if (day < 1 || day > 31)
            {
                AnsiConsole.MarkupLine("[red]Invalid day provided.[/]");
                return null;
            }

            string? timeInput = AskStringOrBack("Enter appointment time (HH:mm)");
            if (timeInput == null) return null;
            string[] timeParts = timeInput.Split(':');
            if (timeParts.Length != 2 ||
                !int.TryParse(timeParts[0], out int hour) ||
                !int.TryParse(timeParts[1], out int minute))
            {
                AnsiConsole.MarkupLine("[red]Invalid time format.[/]");
                return null;
            }
            try
            {
                DateTime appointmentDate = new DateTime(year.Value, month.Value, day.Value, hour, minute, 0);
                return appointmentDate;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Error constructing date: [/]" + ex.Message);
                return null;
            }
        }

        static bool RemoveAppointment()
        {
            int? id = AskIntOrBack("Enter appointment ID to remove");
            if (id == null) return false;
            try
            {
                bool removedFromDB = AppointmentDataAccess.RemoveAppointment(id.Value);
                bool removedFromBST = appointmentBST.RemoveByID(id.Value);
                if (removedFromDB && removedFromBST)
                {
                    AnsiConsole.MarkupLine("[green]Appointment removed successfully.[/]");
                    return true;
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Appointment not found.[/]");
                    return true;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Error removing appointment: [/]" + ex.Message);
                return false;
            }
        }

        static bool SearchAppointment()
        {
            string? patientName = AskStringOrBack("Enter patient name to search");
            if (patientName == null) return false;
            BSTNode? resultNode = appointmentBST.Search(patientName);
            if (resultNode == null)
            {
                AnsiConsole.MarkupLine($"[yellow]No appointments found for patient {patientName}.[/]");
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]Appointments for {patientName}:[/]");
                foreach (var app in resultNode.Appointments)
                    AnsiConsole.MarkupLine(app.ToString());
                return true;
            }
        }

        static bool DisplayAppointmentsTable()
        {
            List<Appointment> appointments = appointmentBST.GetAppointmentsInOrder();
            if (appointments.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No appointments available.[/]");
                return true;
            }

            var table = new Table();
            table.AddColumn(new TableColumn("[u]ID[/]"));
            table.AddColumn(new TableColumn("[u]Patient Name[/]"));
            table.AddColumn(new TableColumn("[u]Dentist Name[/]"));
            table.AddColumn(new TableColumn("[u]Date[/]"));
            table.AddColumn(new TableColumn("[u]Description[/]"));

            foreach (var app in appointments)
            {
                table.AddRow(app.AppointmentID.ToString(),
                             app.PatientName,
                             app.DentistName,
                             app.AppointmentDate.ToString("yyyy-MM-dd HH:mm"),
                             app.Description);
            }
            AnsiConsole.Write(table);
            return true;
        }

        static bool LoadAppointmentsFromFile()
        {
            string? filePath = AskStringOrBack("Enter the path to the text file");
            if (filePath == null) return false;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                AnsiConsole.MarkupLine("[red]Invalid file path or file does not exist.[/]");
                return false;
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
                        AnsiConsole.MarkupLine($"[yellow]Skipping invalid line:[/] {line}");
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

                AnsiConsole.MarkupLine("[green]Appointments successfully loaded from file.[/]");
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Error reading file: [/]" + ex.Message);
                return false;
            }
        }

        static bool RemoveAllAppointments()
        {
            try
            {
                AppointmentDataAccess.RemoveAllAppointments();
                appointmentBST = new AppointmentBST();
                nextAppointmentID = 0;
                AnsiConsole.MarkupLine("[green]All appointments removed. Appointment counter reset to 0.[/]");
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Error removing all appointments: [/]" + ex.Message);
                return false;
            }
        }
    }
}
