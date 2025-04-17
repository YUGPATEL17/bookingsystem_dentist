# Dentist Appointment Management System

A .NET 8 console application for managing dentist appointments, built with Spectre.Console for a rich CLI experience.  
Includes an in‑memory BST for fast searches, SQL Server integration, file‑based import, and full unit/integration/performance testing via MSTest.

---

## Table of Contents

1. [Features](#features)  
2. [Prerequisites](#prerequisites)  
3. [Installation](#installation)  
4. [Makefile Targets](#makefile-targets)  
5. [Building & Running](#building--running)  
6. [Testing](#testing)  
7. [Configuration](#configuration)  
8. [Project Structure](#project-structure)  
9. [Contributing](#contributing)

---

## Features

- **Add / Remove / Search / List** appointments
- **SQL Server** data persistence (MS SQL Express)  
- **File import** to upload bulk appointments 
- **Binary Search Tree** for in‑memory searches (O(log n) average)  
- **Spectre.Console** UI: tables, prompts, status, etc.  
- **Unit, integration & performance tests**  
- **Makefile**–driven build/test workflow & GitHub Actions CI  

