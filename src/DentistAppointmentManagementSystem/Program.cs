using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Spectre.Console;
using DentistAppointmentManagementSystem.Models;
using DentistAppointmentManagementSystem.DataAccess;
using DentistAppointmentManagementSystem.DataStructures;
using DentistAppointmentManagementSystem.Utilities;

#nullable enable

namespace DentistAppointmentManagementSystem
{
    class Program
    {
        static AppointmentBST appointmentBST = new AppointmentBST();
        static int nextAppointmentID = 1;
        static string masterPassword = "admin123";
        static string currentUser = string.Empty;

        static void Main(string[] args)
        {
            while (true)
            {
                int accessLevel = LoginPage();
                LoadAppointmentsFromDatabase();

                if (accessLevel == 1)
                {
                    currentUser = "Admin";
                    AdminMenu();
                }
                else
                {
                    EmployeeMenu();
                }
            }
        }

        static void LoadAppointmentsFromDatabase()
        {
            appointmentBST = new AppointmentBST();
            var appointments = AppointmentDataAccess.GetAllAppointments();
            foreach (var app in appointments)
            {
                appointmentBST.Insert(app);
            }
            nextAppointmentID = appointments.Any() ? appointments.Max(a => a.AppointmentID) + 1 : 1;
        }

        static string? AskStringOrBack(string prompt)
        {
            AnsiConsole.WriteLine(); // Spacer
            string input = AnsiConsole.Prompt(new TextPrompt<string>($"{prompt} (or type 'back' to cancel):"));
            if (input.Trim().Equals("back", StringComparison.OrdinalIgnoreCase))
                return null;
            return input;
        }

        static int? AskIntOrBackWithSpacing(string prompt)
        {
            while (true)
            {
                AnsiConsole.WriteLine(); // Spacer
                string input = AnsiConsole.Prompt(new TextPrompt<string>($"{prompt} (or type 'back' to cancel):"));
                if (input.Trim().Equals("back", StringComparison.OrdinalIgnoreCase))
                    return null;
                if (int.TryParse(input, out int result))
                    return result;
                AnsiConsole.MarkupLine("[red]Invalid input. Please enter a valid number.[/]");
            }
        }

        // Prompts for a name and ensures no digits are present.
        static string? AskNameOrBack(string prompt)
        {
            while (true)
            {
                AnsiConsole.WriteLine(); // Spacer
                string input = AnsiConsole.Prompt(new TextPrompt<string>($"{prompt} (or type 'back' to cancel):"));
                if (input.Trim().Equals("back", StringComparison.OrdinalIgnoreCase))
                    return null;
                if (input.Any(char.IsDigit))
                {
                    AnsiConsole.MarkupLine("[red]Name should not contain numbers. Please enter a valid name.[/]");
                    continue;
                }
                return input;
            }
        }

        static void Pause()
        {
            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }

        static void DisplayHeader()
        {
            string headerArt = @"
██████  ███████ ███    ██ ████████ ██ ██████  ███████ ███████ ██   ██ 
██   ██ ██      ████   ██    ██    ██ ██   ██ ██      ██      ██  ██  
██   ██ █████   ██ ██  ██    ██    ██ ██   ██ █████   ███████ █████   
██   ██ ██      ██  ██ ██    ██    ██ ██   ██ ██           ██ ██  ██  
██████  ███████ ██   ████    ██    ██ ██████  ███████ ███████ ██   ██ 
                                                                     ";
            AnsiConsole.Write(new Markup(headerArt).Centered());
            AnsiConsole.Write(new Markup("[bold underline]Dentist Appointment Management System[/]").Centered());
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                string status = $"Logged in as: {currentUser} | Current Date: {DateTime.Now:yyyy-MM-dd}";
                AnsiConsole.Write(new Rule(status).Centered());
            }
            AnsiConsole.WriteLine();
        }

        static int LoginPage()
        {
            while (true)
            {
                AnsiConsole.Clear();
                string headerArt = @"
██████  ███████ ███    ██ ████████ ██ ██████  ███████ ███████ ██   ██ 
██   ██ ██      ████   ██    ██    ██ ██   ██ ██      ██      ██  ██  
██   ██ █████   ██ ██  ██    ██    ██ ██   ██ █████   ███████ █████   
██   ██ ██      ██  ██ ██    ██    ██ ██   ██ ██           ██ ██  ██  
██████  ███████ ██   ████    ██    ██ ██████  ███████ ███████ ██   ██ 
                                                                     ";
                AnsiConsole.Write(new Markup(headerArt).Centered());
                AnsiConsole.Write(new Markup("[bold underline]Dentist Appointment Management System[/]").Centered());
                AnsiConsole.WriteLine();

                string instructions = "[white](Welcome, use the arrow keys to navigate and press [green]ENTER[/] to select)[/]";
                var instrPanel = new Panel(new Markup(instructions))
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1)
                };
                var instrGrid = new Grid();
                instrGrid.AddColumn(new GridColumn().Centered());
                instrGrid.AddRow(instrPanel);
                AnsiConsole.Write(instrGrid);
                AnsiConsole.WriteLine();

                var loginPrompt = new SelectionPrompt<string>()
                    .Title("[bold yellow]Choose an option:[/]")
                    .PageSize(3)
                    .AddChoices("Login", "Exit Application");
                string option = AnsiConsole.Prompt(loginPrompt);
                if (option.Equals("Exit Application", StringComparison.OrdinalIgnoreCase))
                    Environment.Exit(0);
                else if (option.Equals("Login", StringComparison.OrdinalIgnoreCase))
                {
                    string input = AnsiConsole.Prompt(new TextPrompt<string>("Enter password:").Secret());
                    if (input == masterPassword)
                        return 1;
                    else
                    {
                        var emps = EmployeeDataAccess.GetAllEmployees();
                        Employee? emp = emps.Find(e => e.Password == input);
                        if (emp != null)
                        {
                            currentUser = emp.Name;
                            return 2;
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Incorrect password. Try again.[/]");
                            Thread.Sleep(1500);
                            continue;
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid option. Please try again.[/]");
                    Thread.Sleep(1500);
                    continue;
                }
            }
        }

        static void AdminMenu()
        {
            bool exit = false;
            while (!exit)
            {
                AnsiConsole.Clear();
                DisplayHeader();
                var adminPrompt = new SelectionPrompt<string>()
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
                        "Add a new employee",
                        "Remove Employee", // New option added here.
                        "Reset Password For an Employee",
                        "Log Out",
                        "Exit Application"
                    });
                string choice = AnsiConsole.Prompt(adminPrompt);
                if (choice.Equals("Log Out", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                else if (choice.Equals("Exit Application", StringComparison.OrdinalIgnoreCase))
                {
                    Environment.Exit(0);
                }

                bool completed = choice switch
                {
                    "Add Appointment" => AddAppointment(),
                    "Remove Appointment" => RemoveAppointment(),
                    "Search Appointment by Patient Name" => SearchAppointment(),
                    "Display All Appointments" => DisplayAppointmentsTable(),
                    "Load Appointments from File" => LoadAppointmentsFromFile(),
                    "Remove All Appointments" => RemoveAllAppointments(),
                    "Add a new employee" => AddNewEmployee(),
                    "Remove Employee" => RemoveEmployee(), // Call our new method.
                    "Reset Password For an Employee" => ResetEmployeePassword(),
                    _ => false,
                };
                if (completed)
                    Pause();
            }
        }

        static void EmployeeMenu()
        {
            bool exit = false;
            while (!exit)
            {
                AnsiConsole.Clear();
                DisplayHeader();
                var empPrompt = new SelectionPrompt<string>()
                    .Title("[bold cyan]Select an option:[/]")
                    .PageSize(7)
                    .AddChoices(new[]
                    {
                        "Add Appointment",
                        "Remove Appointment",
                        "Search Appointment by Patient Name",
                        "Display All Appointments",
                        "Log Out",
                        "Exit Application"
                    });
                string choice = AnsiConsole.Prompt(empPrompt);
                if (choice.Equals("Log Out", StringComparison.OrdinalIgnoreCase))
                    return;
                else if (choice.Equals("Exit Application", StringComparison.OrdinalIgnoreCase))
                    Environment.Exit(0);
                bool completed = choice switch
                {
                    "Add Appointment" => AddAppointment(),
                    "Remove Appointment" => RemoveAppointment(),
                    "Search Appointment by Patient Name" => SearchAppointment(),
                    "Display All Appointments" => DisplayAppointmentsTable(),
                    _ => false,
                };
                if (completed)
                    Pause();
            }
        }

        // New method: Removes an employee by ID.
        static bool RemoveEmployee()
        {
            int? id = AskIntOrBackWithSpacing("Enter employee ID to remove");
            if (id == null) return false;
            try
            {
                bool removedFromDB = EmployeeDataAccess.RemoveEmployee(id.Value);
                if (removedFromDB)
                {
                    AnsiConsole.MarkupLine("[green]Employee removed successfully.[/]");
                    return true;
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Employee not found.[/]");
                    return true;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]Error removing employee: [/]" + ex.Message);
                return false;
            }
        }

        static bool AddNewEmployee()
        {
            string? name = AskNameOrBack("Enter new employee name");
            if (name == null) return false;
            string? designation = AskNameOrBack("Enter new employee designation");
            if (designation == null) return false;

            Random rnd = new Random();
            int empId;
            do
            {
                empId = rnd.Next(10000, 100000);
            } while (EmployeeDataAccess.GetAllEmployees().Exists(e => e.EmployeeId == empId));

            int pwdNum = rnd.Next(10000, 100000);
            string pwd = pwdNum.ToString();

            Employee emp = new Employee
            {
                EmployeeId = empId,
                Name = name,
                Designation = designation,
                Password = pwd
            };

            EmployeeDataAccess.InsertEmployee(emp);

            List<Employee> currentEmployees = EmployeeDataAccess.GetAllEmployees();
            var table = TableHelper.CreateRoundedTable();
            table.AddColumn("[u]Employee ID[/]");
            table.AddColumn("[u]Name[/]");
            table.AddColumn("[u]Designation[/]");
            table.AddColumn("[u]Password[/]");
            foreach (var e in currentEmployees)
            {
                table.AddRow(e.EmployeeId.ToString(), e.Name, e.Designation, e.Password);
            }
            AnsiConsole.Write(table);
            return true;
        }

        static bool ResetEmployeePassword()
        {
            List<Employee> currentEmployees = EmployeeDataAccess.GetAllEmployees();
            if (currentEmployees.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No employees available.[/]");
                return true;
            }

            var table = TableHelper.CreateRoundedTable();
            table.AddColumn("[u]Employee ID[/]");
            table.AddColumn("[u]Name[/]");
            table.AddColumn("[u]Designation[/]");
            table.AddColumn("[u]Password[/]");
            foreach (var e in currentEmployees)
            {
                table.AddRow(e.EmployeeId.ToString(), e.Name, e.Designation, e.Password);
            }
            AnsiConsole.Write(table);

            int? id = AskIntOrBackWithSpacing("Enter the Employee ID to reset password");
            if (id == null) return false;

            Employee? emp = currentEmployees.Find(e => e.EmployeeId == id);
            if (emp == null)
            {
                AnsiConsole.MarkupLine("[red]Employee not found.[/]");
                return false;
            }

            Random rnd = new Random();
            int newPwdNum = rnd.Next(10000, 100000);
            string newPwd = newPwdNum.ToString();
            bool updateSuccess = EmployeeDataAccess.UpdateEmployeePassword(emp.EmployeeId, newPwd);
            if (updateSuccess)
                AnsiConsole.MarkupLine($"[green]Password for employee {emp.Name} has been reset to {newPwd}.[/]");
            else
                AnsiConsole.MarkupLine("[red]Failed to update password.[/]");
            currentEmployees = EmployeeDataAccess.GetAllEmployees();
            table = TableHelper.CreateRoundedTable();
            table.AddColumn("[u]Employee ID[/]");
            table.AddColumn("[u]Name[/]");
            table.AddColumn("[u]Designation[/]");
            table.AddColumn("[u]Password[/]");
            foreach (var e in currentEmployees)
            {
                table.AddRow(e.EmployeeId.ToString(), e.Name, e.Designation, e.Password);
            }
            AnsiConsole.Write(table);
            return true;
        }

        static bool AddAppointment()
        {
            string? patientName = AskNameOrBack("Enter patient name");
            if (patientName == null) return false;
            string? dentistName = AskNameOrBack("Enter dentist name");
            if (dentistName == null) return false;
            DateTime? appointmentDate = GetAppointmentDateFromUser();
            if (appointmentDate == null)
            {
                AnsiConsole.MarkupLine("[red]Appointment creation canceled or failed.[/]");
                return false;
            }
            string? description = AskStringOrBack("Enter description");
            if (description == null) return false;
            var newAppointment = new Appointment(nextAppointmentID, patientName, dentistName, appointmentDate.Value, description);
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
            while (true)
            {
                int? year = AskIntOrBackWithSpacing("Enter appointment year (e.g. 2025)");
                if (year == null) return null;
                int currentYear = DateTime.Now.Year;
                if (year < currentYear)
                {
                    AnsiConsole.MarkupLine("[red]Year cannot be in the past. Please enter a valid year.[/]");
                    continue;
                }

                int? month = AskIntOrBackWithSpacing("Enter appointment month (1-12)");
                if (month == null) return null;
                if (month < 1 || month > 12)
                {
                    AnsiConsole.MarkupLine("[red]Invalid month. Please enter a value between 1 and 12.[/]");
                    continue;
                }
                if (year == currentYear && month < DateTime.Now.Month)
                {
                    AnsiConsole.MarkupLine("[red]Month cannot be in the past for the current year. Please try again.[/]");
                    continue;
                }

                int? day = AskIntOrBackWithSpacing("Enter appointment day (1-31)");
                if (day == null) return null;
                if (day < 1 || day > 31)
                {
                    AnsiConsole.MarkupLine("[red]Invalid day. Please enter a value between 1 and 31.[/]");
                    continue;
                }
                if (year == currentYear && month == DateTime.Now.Month && day < DateTime.Now.Day)
                {
                    AnsiConsole.MarkupLine("[red]Day cannot be in the past for the current month and year. Please try again.[/]");
                    continue;
                }

                string? timeInput = AskStringOrBack("Enter appointment time (HH:mm)");
                if (timeInput == null) return null;
                string[] timeParts = timeInput.Split(':');
                if (timeParts.Length != 2 ||
                    !int.TryParse(timeParts[0], out int hour) ||
                    !int.TryParse(timeParts[1], out int minute))
                {
                    AnsiConsole.MarkupLine("[red]Invalid time format. Please enter time in HH:mm format.[/]");
                    continue;
                }
                try
                {
                    DateTime appointmentDate = new DateTime(year.Value, month.Value, day.Value, hour, minute, 0);
                    if (appointmentDate < DateTime.Now)
                    {
                        AnsiConsole.MarkupLine("[red]Appointment date and time cannot be in the past. Please try again.[/]");
                        continue;
                    }
                    return appointmentDate;
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error constructing date: {ex.Message}[/]");
                    continue;
                }
            }
        }

        static bool RemoveAppointment()
        {
            int? id = AskIntOrBackWithSpacing("Enter appointment ID to remove");
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
            string? patientName = AskNameOrBack("Enter patient name to search");
            if (patientName == null) return false;
            var resultNode = appointmentBST.Search(patientName);
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
            var appointments = appointmentBST.GetAppointmentsInOrder();
            if (appointments.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No appointments available.[/]");
                return true;
            }
            var table = TableHelper.CreateRoundedTable();
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
                    var appointment = new Appointment(id, patient, dentist, date, description);
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
                nextAppointmentID = 1;
                AnsiConsole.MarkupLine("[green]All appointments removed. Appointment counter reset to 1.[/]");
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
