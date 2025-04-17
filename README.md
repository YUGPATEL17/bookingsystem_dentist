# DentiDesk - Dentist Appointment Management System

A .NET 8 cross‑platform .NET console application for managing dentist appointments. Built with Spectre.Console for rich text, tables, prompts and live updates.

It supports:

1. **Admin & Employee roles** (login via master/admin password or per‑user credentials)  
2. **CRUD operations** on appointments and employees  
3. **Bulk import** from a text file  
4. **Unit, edge‑case, performance & file‑integration tests** (MSTest)  
5. **CI pipeline** driven by a simple Makefile and GitHub Actions 

---

## Table of Contents

1. [Features](#features)  
2. [Prerequisites](#prerequisites)  
3. [Database](#database)  
4. [Build & Run Locally](#build--run-locally)  
5. [Usage](#usage)  
6. [CI / GitHub Actions](#ci--github-actions)  
7. [Testing](#testing)  

---

## Features

- **Add / Remove / Search / List** appointments
- **SQL Server** data persistence (MS SQL Express)  
- **File import** to upload bulk appointments 
- **Binary Search Tree** for in‑memory searches
- **Spectre.Console** UI : tables, prompts, status, etc.  
- **Unit, integration & performance tests**  
- **Makefile**–driven build/test workflow & GitHub Actions CI  

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)  
- **SQL Server** (e.g. SQL Express / LocalDB)  
- **SQL Server Management Studio (SSMS)** (for running the setup script via GUI)  
- `sqlcmd` CLI tool (optional, if you prefer command‑line)  
- Optional: [make](https://www.gnu.org/software/make/)  

---

## Database

1. Open **SQL Server Management Studio**   
2. Connect to your `localhost\SQLEXPRESS` (or the appropriate instance)  
3. Open `db scripts/createdatabase.sql` and execute it to create the `DentistAppointmentsDB` database and tables

---

## Build & Run Locally

Use the .NET CLI to get up and running:

1. Restore all NuGet packages
dotnet restore DentistAppointmentManagementSystem.sln

2. Build in Release configuration
dotnet build DentistAppointmentManagementSystem.sln -c Release

3. Run the console app
dotnet run --project src/DentistAppointmentManagementSystem/DentistAppointmentManagementSystem.csproj

---

## Usage

    - Launch the app, select role (Admin or Employee).

    - Admin can also add/reset employees and bulk‐remove all appointments.

    - Answer the on‑screen prompts (navigate via arrows + Enter).

    - To import from file, choose “Load Appointments from File” and enter a path to a text file.

---

## CI / GitHub Actions

Our GitHub Actions workflow (.github/workflows/makefile.yml) will automatically:

    - Checkout your code

    - Setup .NET 8

    - Restore | Build | Test (skips integration by default)

Green checks on main mean your non‑integration tests are passing.

---

## Testing

### Unit, Edge‑Case, Performance & File Integration Tests

Run all non‑database tests via the .NET CLI:

```bash
dotnet test DentistAppointmentManagementSystem.sln 
  --filter TestCategory!=Integration
```
### Database Integration Tests

Requires a live SQL Server instance and the DentistAppointmentsDB created:

```
dotnet test DentistAppointmentManagementSystem.sln \
  --filter TestCategory=Integration
```

```
Tip: You can combine filters, e.g. --filter "TestCategory=Integration|EdgeCase", or omit --filter to run everything.
```
---


